create table bpm_af_deployment
(
    id            varchar(64)  not null
        primary key,
    rev          int          null,
    name          varchar(255) null,
    content         longtext     null,
    `remark`              varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`              tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:in use,1:delete',
    `create_user`         varchar(32)                  DEFAULT '' COMMENT 'as its name says',
    `create_time`         timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user`         varchar(32)                  DEFAULT '' COMMENT '更新人',
    `update_time`         timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says'

);


create table bpm_af_taskinst
(
    id             varchar(64)             not null
        primary key,
    proc_def_id    varchar(64)             null,
    task_def_key   varchar(255)            null,
    proc_inst_id   varchar(64)             null,
    execution_id   varchar(64)             null,
    name           varchar(255)            null,
    parent_task_id varchar(64)             null,
    owner          varchar(255)            null,
    assignee       varchar(255)            null,
    assignee_name   varchar(255)            null,
    original_assignee      varchar(255)            null,
    original_assignee_name varchar(255)            null,
    transfer_reason        varchar(1000)           null,
    verify_status          int                     null,
    start_time     datetime(3)             not null,
    claim_time     datetime(3)             null,
    end_time       datetime(3)             null,
    duration       bigint                  null,
    delete_reason  varchar(4000)           null,
    priority       int                     null,
    due_date       datetime(3)             null,
    form_key       varchar(255)            null,
    category       varchar(255)            null,
    tenant_id      varchar(255) default '' null,
    description    varchar(4000)           null,
    update_user       varchar(64)            null,
);

create index AF_HI_TASK_INST_PROCINST
    on bpm_af_taskinst (proc_inst_id);

create index idx_assignee_name
    on bpm_af_taskinst (assignee_name);


create table bpm_af_task
(
    id               varchar(64)             not null
        primary key,
    rev              int                     null,
    execution_id     varchar(64)             null,
    proc_inst_id     varchar(64)             null,
    proc_def_id      varchar(64)             null,
    name             varchar(255)            null,
    parent_task_id   varchar(64)             null,
    task_def_key     varchar(255)            null,
    owner            varchar(255)            null,
    assignee         varchar(255)            null,
    assignee_name     varchar(255)            null,
    delegation       varchar(64)             null,
    priority         int                     null,
    create_time      timestamp(3)            null,
    due_date         datetime(3)             null,
    category         varchar(255)            null,
    suspension_state int                     null,
    tenant_id        varchar(255) default '' null,
    form_key         varchar(255)            null,
    description      varchar(4000)           null
);

create index AF_IDX_TASK_CREATE
    on bpm_af_task (CREATE_TIME);
create index AF_IDX_PROCINSTID
    on bpm_af_task (proc_inst_id);

create table bpm_af_execution
(
    id                   varchar(64)             not null
        primary key,
    rev_                  int                     null,
    proc_inst_id         varchar(64)             null,
    business_key         varchar(255)            null,
    parent_id            varchar(64)             null,
    proc_def_id          varchar(64)             null,
    super_exec           varchar(64)             null,
    root_proc_inst_id    varchar(64)             null,
    act_id               varchar(255)            null,
    is_active            tinyint                 null,
    is_concurrent        tinyint                 null,
    tenant_id            varchar(255) default '' null,
    name                 varchar(255)            null,
    start_time           datetime                null,
    start_user_id        varchar(255)            null,
    is_count_enabled     tinyint                 null,
    evt_subscr_count     int                     null,
    task_count           int                     null,
    var_count            int                     null
);

create index AF_IDX_EXEC_PROCINSTID
    on bpm_af_execution (proc_inst_id);

create index AF_IDX_EXEC_BUSKEY
    on bpm_af_execution (business_key);
