--
-- PostgreSQL database dump
--

\restrict z5DmIjGdg27VF6oakARy2hNwjOsgHELAIEGkwe5xgsbqWYhpakfJHVZvCvwhHvH

-- Dumped from database version 16.14
-- Dumped by pg_dump version 16.14

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

ALTER TABLE ONLY public."RefreshTokens" DROP CONSTRAINT "FK_RefreshTokens_Users_UserId";
ALTER TABLE ONLY public."Notifications" DROP CONSTRAINT "FK_Notifications_Users_UserId";
ALTER TABLE ONLY public."NotificationSettings" DROP CONSTRAINT "FK_NotificationSettings_Users_UserId";
ALTER TABLE ONLY public."Comments" DROP CONSTRAINT "FK_Comments_Users_UserId";
ALTER TABLE ONLY public."Comments" DROP CONSTRAINT "FK_Comments_Comments_ParentCommentId";
ALTER TABLE ONLY public."ActivityLogs" DROP CONSTRAINT "FK_ActivityLogs_Users_UserId";
DROP INDEX public."IX_Users_Username";
DROP INDEX public."IX_Users_Email";
DROP INDEX public."IX_RefreshTokens_UserId_IsRevoked";
DROP INDEX public."IX_RefreshTokens_Token";
DROP INDEX public."IX_Notifications_UserId_IsRead_CreatedAt";
DROP INDEX public."IX_Notifications_ReferenceType_ReferenceId";
DROP INDEX public."IX_Comments_UserId";
DROP INDEX public."IX_Comments_TaskId";
DROP INDEX public."IX_Comments_ParentCommentId";
DROP INDEX public."IX_ActivityLogs_UserId_CreatedAt";
DROP INDEX public."IX_ActivityLogs_TaskId";
DROP INDEX public."IX_ActivityLogs_ProjectId";
ALTER TABLE ONLY public."__EFMigrationsHistory" DROP CONSTRAINT "PK___EFMigrationsHistory";
ALTER TABLE ONLY public."Users" DROP CONSTRAINT "PK_Users";
ALTER TABLE ONLY public."RefreshTokens" DROP CONSTRAINT "PK_RefreshTokens";
ALTER TABLE ONLY public."Notifications" DROP CONSTRAINT "PK_Notifications";
ALTER TABLE ONLY public."NotificationSettings" DROP CONSTRAINT "PK_NotificationSettings";
ALTER TABLE ONLY public."Comments" DROP CONSTRAINT "PK_Comments";
ALTER TABLE ONLY public."ActivityLogs" DROP CONSTRAINT "PK_ActivityLogs";
DROP TABLE public."__EFMigrationsHistory";
DROP TABLE public."Users";
DROP TABLE public."RefreshTokens";
DROP TABLE public."Notifications";
DROP TABLE public."NotificationSettings";
DROP TABLE public."Comments";
DROP TABLE public."ActivityLogs";
SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: ActivityLogs; Type: TABLE; Schema: public; Owner: haiquan
--

CREATE TABLE public."ActivityLogs" (
    "Id" uuid DEFAULT gen_random_uuid() NOT NULL,
    "TaskId" uuid NOT NULL,
    "ProjectId" uuid,
    "UserId" uuid NOT NULL,
    "Action" character varying(50) NOT NULL,
    "Detail" text,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


ALTER TABLE public."ActivityLogs" OWNER TO haiquan;

--
-- Name: Comments; Type: TABLE; Schema: public; Owner: haiquan
--

CREATE TABLE public."Comments" (
    "Id" uuid DEFAULT gen_random_uuid() NOT NULL,
    "TaskId" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Content" text NOT NULL,
    "ParentCommentId" uuid,
    "IsDeleted" boolean DEFAULT false NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."Comments" OWNER TO haiquan;

--
-- Name: NotificationSettings; Type: TABLE; Schema: public; Owner: haiquan
--

CREATE TABLE public."NotificationSettings" (
    "UserId" uuid NOT NULL,
    "OnComment" boolean DEFAULT true NOT NULL,
    "OnAssigned" boolean DEFAULT true NOT NULL,
    "OnStatusChanged" boolean DEFAULT true NOT NULL,
    "OnMention" boolean DEFAULT true NOT NULL,
    "OnSprintStarted" boolean DEFAULT true NOT NULL,
    "OnMemberAdded" boolean DEFAULT true NOT NULL
);


ALTER TABLE public."NotificationSettings" OWNER TO haiquan;

--
-- Name: Notifications; Type: TABLE; Schema: public; Owner: haiquan
--

CREATE TABLE public."Notifications" (
    "Id" uuid DEFAULT gen_random_uuid() NOT NULL,
    "UserId" uuid NOT NULL,
    "Type" character varying(50) NOT NULL,
    "Message" character varying(500) NOT NULL,
    "ReferenceId" uuid NOT NULL,
    "ReferenceType" character varying(20) NOT NULL,
    "IsRead" boolean DEFAULT false NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


ALTER TABLE public."Notifications" OWNER TO haiquan;

--
-- Name: RefreshTokens; Type: TABLE; Schema: public; Owner: haiquan
--

CREATE TABLE public."RefreshTokens" (
    "Id" uuid DEFAULT gen_random_uuid() NOT NULL,
    "UserId" uuid NOT NULL,
    "Token" character varying(500) NOT NULL,
    "ExpiresAt" timestamp with time zone NOT NULL,
    "IsRevoked" boolean DEFAULT false NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


ALTER TABLE public."RefreshTokens" OWNER TO haiquan;

--
-- Name: Users; Type: TABLE; Schema: public; Owner: haiquan
--

CREATE TABLE public."Users" (
    "Id" uuid DEFAULT gen_random_uuid() NOT NULL,
    "Email" character varying(200) NOT NULL,
    "Username" character varying(100) NOT NULL,
    "PasswordHash" character varying(500) NOT NULL,
    "FullName" character varying(200) NOT NULL,
    "AvatarUrl" character varying(500),
    "Role" character varying(20) DEFAULT 'User'::character varying NOT NULL,
    "IsActive" boolean DEFAULT true NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."Users" OWNER TO haiquan;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: haiquan
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO haiquan;

--
-- Data for Name: ActivityLogs; Type: TABLE DATA; Schema: public; Owner: haiquan
--

COPY public."ActivityLogs" ("Id", "TaskId", "ProjectId", "UserId", "Action", "Detail", "CreatedAt") FROM stdin;
b356e137-5621-4ef2-b7d7-6618a53367e5	d1b2c3d4-0004-0004-0004-000000000001	b1b2c3d4-0002-0002-0002-000000000001	a1b2c3d4-0001-0001-0001-000000000001	assigned	Assigned task	2026-05-19 19:48:40.284311+00
6188599e-6dd5-4d6a-be1c-81d028dc01ec	a72ff008-aa43-4fe4-b4b4-6c42cfbd1681	00000000-0000-0000-0000-000000000000	4fae9358-e794-425f-9dca-e545f27a3443	commented	Added a comment on task: hi	2026-06-18 05:06:21.952415+00
a2cc0ba0-e88e-47cf-a397-3ae5f394c7c4	fe186db0-f42b-489c-99d6-73ebe830f18e	\N	a1b2c3d4-0001-0001-0001-000000000001	assigned	Bạn đã được phân công vào công việc: Thiết kế giao diện bằng vueJS	2026-06-18 05:08:37.072833+00
726304e5-cf65-471d-bded-11c1faed625b	a72ff008-aa43-4fe4-b4b4-6c42cfbd1681	00000000-0000-0000-0000-000000000000	4fae9358-e794-425f-9dca-e545f27a3443	commented	Added a comment on task: @duymanh	2026-06-18 05:11:19.98057+00
898a0a15-d1c3-4084-9ad5-f0a476ab0190	9e8974b6-a00e-4caa-91f3-a1914b78b62e	00000000-0000-0000-0000-000000000000	4fae9358-e794-425f-9dca-e545f27a3443	commented	Added a comment on task: @admin	2026-06-18 05:12:06.572954+00
df89ba4e-3dc4-4ed2-ae8b-6deb290cc9d9	a72ff008-aa43-4fe4-b4b4-6c42cfbd1681	00000000-0000-0000-0000-000000000000	4fae9358-e794-425f-9dca-e545f27a3443	commented	Added a comment on task: @admin hi	2026-06-18 05:16:06.184213+00
830838f3-748d-4bd4-b5a5-69905d482101	483c0ac5-3a9b-494a-bd38-91dfe34d8432	00000000-0000-0000-0000-000000000000	4fae9358-e794-425f-9dca-e545f27a3443	commented	Added a comment on task: @manh hi	2026-06-18 05:16:39.86179+00
a306b256-ea8e-4d74-b3ef-e2b85c916689	483c0ac5-3a9b-494a-bd38-91dfe34d8432	00000000-0000-0000-0000-000000000000	4fae9358-e794-425f-9dca-e545f27a3443	commented	Added a comment on task: @manh	2026-06-18 05:17:27.53346+00
b7e75e1b-a37f-4391-afea-a2f3881e9248	483c0ac5-3a9b-494a-bd38-91dfe34d8432	00000000-0000-0000-0000-000000000000	4fae9358-e794-425f-9dca-e545f27a3443	commented	Added a comment on task: @admin	2026-06-18 05:18:29.330227+00
37f33d18-b6e4-41aa-859e-dda0c4e6cd4a	fe186db0-f42b-489c-99d6-73ebe830f18e	00000000-0000-0000-0000-000000000000	a1b2c3d4-0001-0001-0001-000000000001	commented	Added a comment on task: @duymanh  test comment	2026-06-18 05:22:53.743688+00
397e67bb-ce12-49a6-95be-733b9ac25d3a	6ec9a842-3b26-4021-9583-c113617e5d6a	00000000-0000-0000-0000-000000000000	4fae9358-e794-425f-9dca-e545f27a3443	commented	Added a comment on task: @admin	2026-06-18 06:41:50.329047+00
c74b1dac-2128-40c3-a4b5-8c8fa9e89d37	6ec9a842-3b26-4021-9583-c113617e5d6a	00000000-0000-0000-0000-000000000000	a1b2c3d4-0001-0001-0001-000000000001	commented	Added a comment on task: Gọi gì tôi	2026-06-18 06:42:09.999562+00
7293df2a-3fb8-490b-8399-0eca2ccaf625	6ec9a842-3b26-4021-9583-c113617e5d6a	00000000-0000-0000-0000-000000000000	020585f0-6265-4c56-ab69-7b6355165347	commented	Added a comment on task: Hello	2026-06-18 06:42:42.445126+00
069bdf4a-1015-4a95-a853-52d3ec10e0e3	d1b2c3d4-0004-0004-0004-000000000001	00000000-0000-0000-0000-000000000000	d06b596e-a87b-4090-a1bb-9d2bd40e2a01	commented	Added a comment on task: Automated test comment by user_1483	2026-06-18 06:55:09.531278+00
97c4c545-fba9-4895-808d-4d3e73a91d48	fadf8c2a-cee0-4a77-b35e-b8e93c5d2fd6	00000000-0000-0000-0000-000000000000	020585f0-6265-4c56-ab69-7b6355165347	commented	Added a comment on task: hreheh	2026-06-18 07:31:27.888552+00
b16657e6-95b5-4221-9f04-93dcfd7f097d	24902389-f2bc-434d-aaa9-8ece819864b1	00000000-0000-0000-0000-000000000000	a1b2c3d4-0001-0001-0001-000000000001	commented	Added a comment on task: Test comment	2026-06-18 07:40:10.311686+00
e3eab376-654c-402b-8891-b3533c431be9	1460c199-90b3-49be-a120-5e298e3e3214	00000000-0000-0000-0000-000000000000	a1b2c3d4-0001-0001-0001-000000000001	commented	Added a comment on task: hi	2026-06-18 07:43:28.127592+00
736e1ad0-d737-4256-9106-28d2732959d3	1460c199-90b3-49be-a120-5e298e3e3214	00000000-0000-0000-0000-000000000000	a1b2c3d4-0001-0001-0001-000000000001	commented	Added a comment on task: Test comment from admin	2026-06-18 07:49:33.879883+00
949b6cf1-055a-477a-a69e-90971b41a177	24902389-f2bc-434d-aaa9-8ece819864b1	00000000-0000-0000-0000-000000000000	4fae9358-e794-425f-9dca-e545f27a3443	commented	Added a comment on task: hi	2026-06-18 07:51:44.257975+00
\.


--
-- Data for Name: Comments; Type: TABLE DATA; Schema: public; Owner: haiquan
--

COPY public."Comments" ("Id", "TaskId", "UserId", "Content", "ParentCommentId", "IsDeleted", "CreatedAt", "UpdatedAt") FROM stdin;
2e0757ea-a14b-404d-859c-bf704e5a20e8	d1b2c3d4-0004-0004-0004-000000000001	a1b2c3d4-0001-0001-0001-000000000001	Hello world	\N	f	2026-06-07 19:48:40.284311+00	\N
76562711-991d-4d40-b194-79ca534837ee	d1b2c3d4-0004-0004-0004-000000000002	a1b2c3d4-0001-0001-0001-000000000002	Test comment	\N	f	2026-06-12 19:48:40.284311+00	\N
3bc44708-b80a-4ec2-bbf6-08f2e49f3f17	a72ff008-aa43-4fe4-b4b4-6c42cfbd1681	4fae9358-e794-425f-9dca-e545f27a3443	hi	\N	f	2026-06-18 05:06:21.821669+00	\N
6c724b77-7959-4ab0-a3e3-beb803b62b91	a72ff008-aa43-4fe4-b4b4-6c42cfbd1681	4fae9358-e794-425f-9dca-e545f27a3443	@duymanh	\N	f	2026-06-18 05:11:19.979623+00	\N
13bb46eb-baa6-4354-90cc-a143c3991297	9e8974b6-a00e-4caa-91f3-a1914b78b62e	4fae9358-e794-425f-9dca-e545f27a3443	@admin	\N	f	2026-06-18 05:12:06.571923+00	\N
3bc1f4de-a1db-4b4c-a225-67467d1efb2a	a72ff008-aa43-4fe4-b4b4-6c42cfbd1681	4fae9358-e794-425f-9dca-e545f27a3443	@admin hi	\N	f	2026-06-18 05:16:06.18332+00	\N
144cda2f-3ec5-4f85-a4cc-3b9581f2d605	483c0ac5-3a9b-494a-bd38-91dfe34d8432	4fae9358-e794-425f-9dca-e545f27a3443	@manh hi	\N	f	2026-06-18 05:16:39.861152+00	\N
2c59a6e7-df8a-406d-960a-226ee4ebffb1	483c0ac5-3a9b-494a-bd38-91dfe34d8432	4fae9358-e794-425f-9dca-e545f27a3443	@manh	\N	f	2026-06-18 05:17:27.53275+00	\N
003bd997-56d6-4c2a-8029-d27e8968177f	483c0ac5-3a9b-494a-bd38-91dfe34d8432	4fae9358-e794-425f-9dca-e545f27a3443	@admin	\N	f	2026-06-18 05:18:29.329417+00	\N
6a402417-d041-4473-ad93-6bb8f0a971f6	fe186db0-f42b-489c-99d6-73ebe830f18e	a1b2c3d4-0001-0001-0001-000000000001	@duymanh  test comment	\N	f	2026-06-18 05:22:53.66051+00	\N
a270e312-d13e-48b7-8157-73d241dd36c3	6ec9a842-3b26-4021-9583-c113617e5d6a	4fae9358-e794-425f-9dca-e545f27a3443	@admin	\N	f	2026-06-18 06:41:50.327985+00	\N
1a65bb41-dcc5-410c-a583-57494ef78ef9	6ec9a842-3b26-4021-9583-c113617e5d6a	a1b2c3d4-0001-0001-0001-000000000001	Gọi gì tôi	\N	f	2026-06-18 06:42:09.998822+00	\N
cec82e04-e1de-4bc9-ae1a-071d13393697	6ec9a842-3b26-4021-9583-c113617e5d6a	020585f0-6265-4c56-ab69-7b6355165347	Hello	\N	f	2026-06-18 06:42:42.444207+00	\N
bc86caa4-c207-4564-9b9e-3c14e4437469	d1b2c3d4-0004-0004-0004-000000000001	d06b596e-a87b-4090-a1bb-9d2bd40e2a01	Automated test comment by user_1483	\N	f	2026-06-18 06:55:09.53044+00	\N
b159c1d3-f5cb-457f-9c80-3f041fc1b4ee	fadf8c2a-cee0-4a77-b35e-b8e93c5d2fd6	020585f0-6265-4c56-ab69-7b6355165347	hreheh	\N	f	2026-06-18 07:31:27.885729+00	\N
07058c0d-0016-4049-a78c-19376291d9ec	1460c199-90b3-49be-a120-5e298e3e3214	a1b2c3d4-0001-0001-0001-000000000001	hi	\N	f	2026-06-18 07:43:28.125381+00	\N
0efa0aef-ccaf-4ab8-ad07-d066995a993a	1460c199-90b3-49be-a120-5e298e3e3214	a1b2c3d4-0001-0001-0001-000000000001	Test comment from admin	\N	f	2026-06-18 07:49:33.87823+00	\N
34ca033b-bdea-4f33-a8e2-834ba350ce05	24902389-f2bc-434d-aaa9-8ece819864b1	a1b2c3d4-0001-0001-0001-000000000001	Test commentttt	\N	f	2026-06-18 07:40:10.27278+00	2026-06-18 07:51:45.106905+00
8f0c11b9-bfa8-448e-ab55-b7a622e302eb	24902389-f2bc-434d-aaa9-8ece819864b1	4fae9358-e794-425f-9dca-e545f27a3443	hello	\N	t	2026-06-18 07:51:44.25677+00	2026-06-18 07:51:52.562691+00
\.


--
-- Data for Name: NotificationSettings; Type: TABLE DATA; Schema: public; Owner: haiquan
--

COPY public."NotificationSettings" ("UserId", "OnComment", "OnAssigned", "OnStatusChanged", "OnMention", "OnSprintStarted", "OnMemberAdded") FROM stdin;
a1b2c3d4-0001-0001-0001-000000000001	t	t	t	t	t	t
a1b2c3d4-0001-0001-0001-000000000002	t	t	t	t	t	t
a1b2c3d4-0001-0001-0001-000000000003	t	t	t	t	t	t
39d02c92-b790-4125-bc7e-e9f83692c50e	t	t	t	t	t	t
4fae9358-e794-425f-9dca-e545f27a3443	t	t	t	t	t	t
ab9961d5-dd86-4c12-bee0-e5c04f475679	t	t	t	t	t	t
f2ed7299-cb1c-44b7-bb1d-b3e3e5771973	t	t	t	t	t	t
461f4e18-8290-4b61-b441-b479ac70e2bb	t	t	t	t	t	t
04c64c88-01f9-4975-a7b6-57fd6e77a9eb	t	t	t	t	t	t
0eb1da64-efc2-4467-965e-c31c89d2f03c	t	t	t	t	t	t
51b8f7ac-e640-4d76-a317-2c58be9f2a38	t	t	t	t	t	t
0d791acd-0975-4a1c-b392-e35710fa2f9e	t	t	t	t	t	t
4fb1d1d7-1dc9-4694-a1e8-d27f84127927	t	t	t	t	t	t
2ee6d169-128b-466a-affb-1605609e832e	t	t	t	t	t	t
3285ed92-706d-4d69-8e55-df6d8567ed91	t	t	t	t	t	t
d4008de2-0554-47e4-8fb5-e87ea2b8cf19	t	t	t	t	t	t
020585f0-6265-4c56-ab69-7b6355165347	t	t	t	t	t	t
d89d18dd-5082-4178-8108-f3973141213d	t	t	t	t	t	t
d06b596e-a87b-4090-a1bb-9d2bd40e2a01	t	t	t	t	t	t
15d2e4c1-5eac-4017-95c3-d02d33c7ea5a	t	t	t	t	t	t
699b9285-7105-4c1c-bf18-2bcd7daebcc7	t	t	t	t	t	t
\.


--
-- Data for Name: Notifications; Type: TABLE DATA; Schema: public; Owner: haiquan
--

COPY public."Notifications" ("Id", "UserId", "Type", "Message", "ReferenceId", "ReferenceType", "IsRead", "CreatedAt") FROM stdin;
ce5981ca-e8ea-4b98-8a07-f40104e6f8d8	a1b2c3d4-0001-0001-0001-000000000002	task.assigned	Assigned task	d1b2c3d4-0004-0004-0004-000000000001	Task	t	2026-05-19 19:48:40.284311+00
ac4971fe-c825-4f5c-8b45-12bc1537f456	a1b2c3d4-0001-0001-0001-000000000002	task.deadline.approaching	Công việc của bạn sắp đến hạn hoàn thành: Thiết kế giao diện bằng vueJS	fe186db0-f42b-489c-99d6-73ebe830f18e	Task	t	2026-06-18 05:09:11.223915+00
82904159-38d3-4545-9bce-22fc0662189f	a1b2c3d4-0001-0001-0001-000000000002	task.assigned	Bạn đã được phân công vào công việc: Thiết kế giao diện bằng vueJS	fe186db0-f42b-489c-99d6-73ebe830f18e	Task	t	2026-06-18 05:08:37.072833+00
5d93d336-8413-46d0-82c1-6bb41494da61	a1b2c3d4-0001-0001-0001-000000000001	comment.mention	Bạn đã được nhắc đến trong một bình luận công việc.	9e8974b6-a00e-4caa-91f3-a1914b78b62e	Task	t	2026-06-18 05:12:06.579255+00
57f8fb02-b0ff-4446-b138-2ff5d64b2c73	a1b2c3d4-0001-0001-0001-000000000001	comment.mention	Bạn đã được nhắc đến trong một bình luận công việc.	a72ff008-aa43-4fe4-b4b4-6c42cfbd1681	Task	t	2026-06-18 05:16:06.205175+00
887abbdd-7bea-4a62-bd96-757391d837c7	a1b2c3d4-0001-0001-0001-000000000002	comment.mention	Bạn đã được nhắc đến trong một bình luận công việc.	a72ff008-aa43-4fe4-b4b4-6c42cfbd1681	Task	t	2026-06-18 05:11:20.009702+00
b62c4f2d-1e04-4ac9-be98-ba4a7a404323	a1b2c3d4-0001-0001-0001-000000000001	comment.mention	Bạn đã được nhắc đến trong một bình luận công việc.	483c0ac5-3a9b-494a-bd38-91dfe34d8432	Task	t	2026-06-18 05:18:29.333755+00
1c510c57-672c-48d2-a663-42964013ecca	a1b2c3d4-0001-0001-0001-000000000002	comment.mention	Bạn đã được nhắc đến trong một bình luận công việc.	fe186db0-f42b-489c-99d6-73ebe830f18e	Task	t	2026-06-18 05:22:53.969749+00
255329da-4778-424f-a899-75402d91b613	a1b2c3d4-0001-0001-0001-000000000001	comment.mention	Bạn đã được nhắc đến trong một bình luận công việc.	6ec9a842-3b26-4021-9583-c113617e5d6a	Task	t	2026-06-18 06:41:50.340292+00
\.


--
-- Data for Name: RefreshTokens; Type: TABLE DATA; Schema: public; Owner: haiquan
--

COPY public."RefreshTokens" ("Id", "UserId", "Token", "ExpiresAt", "IsRevoked", "CreatedAt") FROM stdin;
\.


--
-- Data for Name: Users; Type: TABLE DATA; Schema: public; Owner: haiquan
--

COPY public."Users" ("Id", "Email", "Username", "PasswordHash", "FullName", "AvatarUrl", "Role", "IsActive", "CreatedAt", "UpdatedAt") FROM stdin;
a1b2c3d4-0001-0001-0001-000000000001	admin@gmail.com	admin	AIxwOS46v70PpHu8LtlqqZvUnhWXJ/y6Dy5qvrOp1gE=	Nguyễn Văn Admin	\N	Admin	t	2026-04-18 19:48:40.284311+00	\N
a1b2c3d4-0001-0001-0001-000000000002	manh.nguyen@gmail.com	duymanh	AIxwOS46v70PpHu8LtlqqZvUnhWXJ/y6Dy5qvrOp1gE=	Nguyễn Duy Mạnh	\N	User	t	2026-04-23 19:48:40.284311+00	\N
a1b2c3d4-0001-0001-0001-000000000003	linh.tran@gmail.com	tranailinh	AIxwOS46v70PpHu8LtlqqZvUnhWXJ/y6Dy5qvrOp1gE=	Trần Ái Linh	\N	User	t	2026-04-28 19:48:40.284311+00	\N
39d02c92-b790-4125-bc7e-e9f83692c50e	final_shared@gmail.com	test_final_shared	/8EhoiEJWL905ah0Zo89l40ktqgkFJbM/zwOokXk8SY=	Shared Test	\N	User	t	2026-06-17 19:48:51.547761+00	\N
4fae9358-e794-425f-9dca-e545f27a3443	manhdzskt00@gmail.com	manh	lsrjXOipsCRBeL8o5JZsLOG4OFcjqWprg4hYzdbKCh4=	manh	\N	User	t	2026-06-17 19:50:02.674408+00	\N
ab9961d5-dd86-4c12-bee0-e5c04f475679	kkk@gmail.com	A11	KW+JYAg126benpU8czPW8FPkmoQHOBagkdhKMPll0mI=	Nguyen van A	\N	User	t	2026-06-17 19:50:17.564448+00	\N
f2ed7299-cb1c-44b7-bb1d-b3e3e5771973	1a@gmail.com	H1	lsrjXOipsCRBeL8o5JZsLOG4OFcjqWprg4hYzdbKCh4=	Nguyễn Văn Hoang	\N	User	t	2026-06-18 02:04:54.231443+00	\N
461f4e18-8290-4b61-b441-b479ac70e2bb	leomessi@gmail.com	leomessi	LVnY3NP7vfStV9zcajSrIqVtB96ryozdIB2LP1vMuhs=	Leo Messi	\N	User	t	2026-06-18 02:27:05.697494+00	\N
04c64c88-01f9-4975-a7b6-57fd6e77a9eb	leminhhieu@gmail.com	leminhhieu	50hfav4uylOa1YbRBDDfQP8bMAwAFx+pCdMUdlcOa8I=	Lê Minh Hiếu	\N	User	t	2026-06-18 02:27:51.348871+00	\N
0eb1da64-efc2-4467-965e-c31c89d2f03c	nguyenvana@gmail.com	nguyenvana	zJSyTgvUMNpj7uRk62G9F2pCFFTbvhmD5pqpgiIXbBw=	Nguyễn Văn A	\N	User	t	2026-06-18 02:28:44.359755+00	\N
51b8f7ac-e640-4d76-a317-2c58be9f2a38	manh@gmail.com	manh12	WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=	manh	\N	User	t	2026-06-18 02:33:39.900657+00	\N
0d791acd-0975-4a1c-b392-e35710fa2f9e	test1@gmail.com	test	qY7FxQRIAMiOhi8Ae5jYmBX8QMoVXWznkJUw15LpCc4=	test	\N	User	t	2026-06-18 03:40:39.841869+00	\N
4fb1d1d7-1dc9-4694-a1e8-d27f84127927	t@gmail.com	test1	lsrjXOipsCRBeL8o5JZsLOG4OFcjqWprg4hYzdbKCh4=	Nguyễn Văn Thắng	\N	User	t	2026-06-18 04:22:16.457254+00	\N
2ee6d169-128b-466a-affb-1605609e832e	u@gmail.com	T1	lsrjXOipsCRBeL8o5JZsLOG4OFcjqWprg4hYzdbKCh4=	Nguyễn Văn Tinh	\N	User	t	2026-06-18 05:28:33.236069+00	\N
3285ed92-706d-4d69-8e55-df6d8567ed91	manhdzskt11@gmail.com	duy	lsrjXOipsCRBeL8o5JZsLOG4OFcjqWprg4hYzdbKCh4=	duy	\N	User	t	2026-06-18 06:39:43.947545+00	\N
d4008de2-0554-47e4-8fb5-e87ea2b8cf19	example@gmail.com	bocuabanbg00	uV5E9o6wfyAP2XI53LEQfI8cK9auGXSW028G1tw16wI=	Nguyễn Chí Linh	\N	User	t	2026-06-18 06:40:18.640242+00	\N
020585f0-6265-4c56-ab69-7b6355165347	hehe@gmail.com	nguyenvanb	jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=	Nguyen Van B	\N	User	t	2026-06-18 06:40:51.166325+00	\N
d89d18dd-5082-4178-8108-f3973141213d	string	string	RzKH+CmNunFjqJeQiVj3wOrnM+JdLgJ5kuou3JvtL6g=	string	\N	User	t	2026-06-18 06:49:35.349025+00	\N
d06b596e-a87b-4090-a1bb-9d2bd40e2a01	user_1483@gmail.com	user_1483	AIxwOS46v70PpHu8LtlqqZvUnhWXJ/y6Dy5qvrOp1gE=	Test User 1483	\N	User	t	2026-06-18 06:55:09.25259+00	\N
15d2e4c1-5eac-4017-95c3-d02d33c7ea5a	quang@gmail.com	quang	lsrjXOipsCRBeL8o5JZsLOG4OFcjqWprg4hYzdbKCh4=	Nguyễn Quang	\N	User	t	2026-06-18 06:56:46.795026+00	\N
699b9285-7105-4c1c-bf18-2bcd7daebcc7	h1@gmail.com	H123	lsrjXOipsCRBeL8o5JZsLOG4OFcjqWprg4hYzdbKCh4=	Nguyễn Văn Hoà	\N	User	t	2026-06-18 06:58:12.397293+00	\N
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: haiquan
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20260615091049_InitialCreate	10.0.9
\.


--
-- Name: ActivityLogs PK_ActivityLogs; Type: CONSTRAINT; Schema: public; Owner: haiquan
--

ALTER TABLE ONLY public."ActivityLogs"
    ADD CONSTRAINT "PK_ActivityLogs" PRIMARY KEY ("Id");


--
-- Name: Comments PK_Comments; Type: CONSTRAINT; Schema: public; Owner: haiquan
--

ALTER TABLE ONLY public."Comments"
    ADD CONSTRAINT "PK_Comments" PRIMARY KEY ("Id");


--
-- Name: NotificationSettings PK_NotificationSettings; Type: CONSTRAINT; Schema: public; Owner: haiquan
--

ALTER TABLE ONLY public."NotificationSettings"
    ADD CONSTRAINT "PK_NotificationSettings" PRIMARY KEY ("UserId");


--
-- Name: Notifications PK_Notifications; Type: CONSTRAINT; Schema: public; Owner: haiquan
--

ALTER TABLE ONLY public."Notifications"
    ADD CONSTRAINT "PK_Notifications" PRIMARY KEY ("Id");


--
-- Name: RefreshTokens PK_RefreshTokens; Type: CONSTRAINT; Schema: public; Owner: haiquan
--

ALTER TABLE ONLY public."RefreshTokens"
    ADD CONSTRAINT "PK_RefreshTokens" PRIMARY KEY ("Id");


--
-- Name: Users PK_Users; Type: CONSTRAINT; Schema: public; Owner: haiquan
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT "PK_Users" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: haiquan
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: IX_ActivityLogs_ProjectId; Type: INDEX; Schema: public; Owner: haiquan
--

CREATE INDEX "IX_ActivityLogs_ProjectId" ON public."ActivityLogs" USING btree ("ProjectId");


--
-- Name: IX_ActivityLogs_TaskId; Type: INDEX; Schema: public; Owner: haiquan
--

CREATE INDEX "IX_ActivityLogs_TaskId" ON public."ActivityLogs" USING btree ("TaskId");


--
-- Name: IX_ActivityLogs_UserId_CreatedAt; Type: INDEX; Schema: public; Owner: haiquan
--

CREATE INDEX "IX_ActivityLogs_UserId_CreatedAt" ON public."ActivityLogs" USING btree ("UserId", "CreatedAt");


--
-- Name: IX_Comments_ParentCommentId; Type: INDEX; Schema: public; Owner: haiquan
--

CREATE INDEX "IX_Comments_ParentCommentId" ON public."Comments" USING btree ("ParentCommentId");


--
-- Name: IX_Comments_TaskId; Type: INDEX; Schema: public; Owner: haiquan
--

CREATE INDEX "IX_Comments_TaskId" ON public."Comments" USING btree ("TaskId");


--
-- Name: IX_Comments_UserId; Type: INDEX; Schema: public; Owner: haiquan
--

CREATE INDEX "IX_Comments_UserId" ON public."Comments" USING btree ("UserId");


--
-- Name: IX_Notifications_ReferenceType_ReferenceId; Type: INDEX; Schema: public; Owner: haiquan
--

CREATE INDEX "IX_Notifications_ReferenceType_ReferenceId" ON public."Notifications" USING btree ("ReferenceType", "ReferenceId");


--
-- Name: IX_Notifications_UserId_IsRead_CreatedAt; Type: INDEX; Schema: public; Owner: haiquan
--

CREATE INDEX "IX_Notifications_UserId_IsRead_CreatedAt" ON public."Notifications" USING btree ("UserId", "IsRead", "CreatedAt");


--
-- Name: IX_RefreshTokens_Token; Type: INDEX; Schema: public; Owner: haiquan
--

CREATE UNIQUE INDEX "IX_RefreshTokens_Token" ON public."RefreshTokens" USING btree ("Token");


--
-- Name: IX_RefreshTokens_UserId_IsRevoked; Type: INDEX; Schema: public; Owner: haiquan
--

CREATE INDEX "IX_RefreshTokens_UserId_IsRevoked" ON public."RefreshTokens" USING btree ("UserId", "IsRevoked");


--
-- Name: IX_Users_Email; Type: INDEX; Schema: public; Owner: haiquan
--

CREATE UNIQUE INDEX "IX_Users_Email" ON public."Users" USING btree ("Email");


--
-- Name: IX_Users_Username; Type: INDEX; Schema: public; Owner: haiquan
--

CREATE UNIQUE INDEX "IX_Users_Username" ON public."Users" USING btree ("Username");


--
-- Name: ActivityLogs FK_ActivityLogs_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: haiquan
--

ALTER TABLE ONLY public."ActivityLogs"
    ADD CONSTRAINT "FK_ActivityLogs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE RESTRICT;


--
-- Name: Comments FK_Comments_Comments_ParentCommentId; Type: FK CONSTRAINT; Schema: public; Owner: haiquan
--

ALTER TABLE ONLY public."Comments"
    ADD CONSTRAINT "FK_Comments_Comments_ParentCommentId" FOREIGN KEY ("ParentCommentId") REFERENCES public."Comments"("Id") ON DELETE RESTRICT;


--
-- Name: Comments FK_Comments_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: haiquan
--

ALTER TABLE ONLY public."Comments"
    ADD CONSTRAINT "FK_Comments_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE RESTRICT;


--
-- Name: NotificationSettings FK_NotificationSettings_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: haiquan
--

ALTER TABLE ONLY public."NotificationSettings"
    ADD CONSTRAINT "FK_NotificationSettings_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- Name: Notifications FK_Notifications_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: haiquan
--

ALTER TABLE ONLY public."Notifications"
    ADD CONSTRAINT "FK_Notifications_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- Name: RefreshTokens FK_RefreshTokens_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: haiquan
--

ALTER TABLE ONLY public."RefreshTokens"
    ADD CONSTRAINT "FK_RefreshTokens_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

\unrestrict z5DmIjGdg27VF6oakARy2hNwjOsgHELAIEGkwe5xgsbqWYhpakfJHVZvCvwhHvH

