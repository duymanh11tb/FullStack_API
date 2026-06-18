using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace QLDA_PCCV.Services
{
    public class ExternalServiceClient : IExternalServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _projectServiceUrl;
        private readonly string _taskServiceUrl;
        private readonly bool _bypassChecks;
        private readonly ILogger<ExternalServiceClient> _logger;

        public ExternalServiceClient(HttpClient httpClient, IConfiguration configuration, ILogger<ExternalServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _projectServiceUrl = configuration["ApiUrls:ProjectApi"] ?? "http://103.178.235.78:5001";
            _taskServiceUrl = configuration["ApiUrls:TaskApi"] ?? "http://103.178.235.78:5002";
            _bypassChecks = configuration.GetValue<bool>("ApiUrls:BypassExternalChecks");
        }

        public async Task<TaskItemDto?> GetTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
        {
            if (_bypassChecks)
            {
                _logger.LogWarning("Bypassing external task check. Returning mock task details.");
                return new TaskItemDto
                {
                    TaskId = taskId,
                    BoardId = Guid.Empty, // Maps to a dummy ProjectId
                    Title = "Bypassed Task"
                };
            }

            try
            {
                var url = $"{_taskServiceUrl.TrimEnd('/')}/api/Task/{taskId}";
                var response = await _httpClient.GetAsync(url, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TaskItemDto>(cancellationToken: cancellationToken);
                }
                _logger.LogWarning("Task {TaskId} not found or failed in TaskService. Status: {StatusCode}", taskId, response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching task {TaskId} from external service", taskId);
            }

            return null;
        }

        public async Task<List<ProjectMemberDto>?> GetProjectMembersAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            if (_bypassChecks)
            {
                _logger.LogWarning("Bypassing external project members check. Returning empty list.");
                return new List<ProjectMemberDto>();
            }

            try
            {
                var url = $"{_projectServiceUrl.TrimEnd('/')}/api/projects/{projectId}/Members";
                var response = await _httpClient.GetAsync(url, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    // Try to read as wrapped {"data": [...]} response
                    try 
                    {
                        var wrapped = await response.Content.ReadFromJsonAsync<WrappedMembersResponse>(cancellationToken: cancellationToken);
                        if (wrapped?.Data != null)
                        {
                            return wrapped.Data;
                        }
                    }
                    catch
                    {
                        // Fallback: try direct array
                        return await response.Content.ReadFromJsonAsync<List<ProjectMemberDto>>(cancellationToken: cancellationToken);
                    }
                }
                _logger.LogWarning("Failed to fetch project members for project {ProjectId}. Status: {StatusCode}", projectId, response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching members for project {ProjectId}", projectId);
            }

            return null;
        }

        public async Task<bool> IsUserInProjectAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default)
        {
            if (_bypassChecks) return true;

            var members = await GetProjectMembersAsync(projectId, cancellationToken);
            if (members == null) return false;

            return members.Any(m => m.UserId == userId || m.Id == userId);
        }

        public async Task<int?> GetUserRoleInProjectAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default)
        {
            if (_bypassChecks) return 2; // Default to Member role

            var members = await GetProjectMembersAsync(projectId, cancellationToken);
            if (members == null) return null;

            var member = members.FirstOrDefault(m => m.UserId == userId || m.Id == userId);
            return member?.Role;
        }

        private class WrappedMembersResponse
        {
            public List<ProjectMemberDto>? Data { get; set; }
        }
    }
}
