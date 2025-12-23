CREATE TABLE t_bpmn_conf
(
    id                  BIGINT IDENTITY(1,1) NOT NULL,
    bpmn_code           NVARCHAR(60)      DEFAULT N'',
    bpmn_name           NVARCHAR(60)      DEFAULT N'',
    bpmn_type           INT,
    form_code           NVARCHAR(100)     DEFAULT N'',
    app_id              INT,
    deduplication_type  INT      NOT NULL DEFAULT 1,
    effective_status    INT      NOT NULL DEFAULT 0,
    is_all              INT      NOT NULL DEFAULT 0,
    is_out_side_process INT               DEFAULT 0,
    is_lowcode_flow     INT               DEFAULT 0,
    business_party_id   INT,
    extra_flags         INT,
    remark              NVARCHAR(255)     DEFAULT N'',
    tenant_id           NVARCHAR(64)      DEFAULT N'',
    is_del              TINYINT  NOT NULL DEFAULT 0,
    create_user         NVARCHAR(32)      DEFAULT N'',
    create_time         DATETIME NOT NULL DEFAULT GETDATE(),
    update_user         NVARCHAR(32)      DEFAULT N'',
    update_time         DATETIME          DEFAULT GETDATE(),

    -- 主键
    CONSTRAINT PK_t_bpmn_conf PRIMARY KEY (id),

    -- 唯一约束（替代 UNIQUE KEY）
    CONSTRAINT UK_t_bpmn_conf_bpmn_code UNIQUE (bpmn_code)
);

CREATE TABLE t_bpmn_node
(

    id                   BIGINT IDENTITY(1,1) NOT NULL,
    conf_id              BIGINT   NOT NULL,
    node_id              NVARCHAR(60)         DEFAULT '',
    node_type            INT      NOT NULL,
    node_property        INT      NOT NULL,
    node_from            NVARCHAR(60)         DEFAULT '',
    node_froms           NVARCHAR(255),
    batch_status         INT      NOT NULL DEFAULT 0,
    approval_standard    INT      NOT NULL DEFAULT 2,
    node_name            NVARCHAR(255),
    node_display_name    NVARCHAR(255)        DEFAULT '',
    annotation           NVARCHAR(255),
    is_deduplication     INT      NOT NULL DEFAULT 0,
    deduplicationExclude INT               DEFAULT 0,
    is_dynamicCondition  INT               DEFAULT 0,
    is_parallel          INT               DEFAULT 0,
    is_sign_up           INT      NOT NULL DEFAULT 0,
    no_header_action     TINYINT,
    remark               NVARCHAR(255)        DEFAULT '',
    tenant_id            NVARCHAR(64)         DEFAULT '',
    is_del               TINYINT  NOT NULL DEFAULT 0,
    create_user          NVARCHAR(50)         DEFAULT '',
    create_time          DATETIME NOT NULL DEFAULT GETDATE(),
    update_user          NVARCHAR(50)         DEFAULT '',
    update_time          DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpmn_node PRIMARY KEY (id)

);


CREATE
NONCLUSTERED INDEX index_conf_id

 ON t_bpmn_node (conf_id);

CREATE TABLE t_bpmn_node_business_table_conf
(
    id                       INT IDENTITY(1,1) NOT NULL,
    bpmn_node_id             BIGINT   NOT NULL,
    configuration_table_type INT,
    table_field_type         INT,
    sign_type                INT      NOT NULL,
    remark                   NVARCHAR(255)     DEFAULT '',
    tenant_id                NVARCHAR(64)      DEFAULT '',
    is_del                   INT      NOT NULL DEFAULT 0,
    create_user              NVARCHAR(50)      DEFAULT '',
    create_time              DATETIME NOT NULL DEFAULT GETDATE(),
    update_user              NVARCHAR(50)      DEFAULT '',
    update_time              DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpmn_node_business_table_conf PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX index_node_id
    ON t_bpmn_node_business_table_conf (bpmn_node_id);


CREATE TABLE t_bpmn_conf_notice_template
(
    id          INT IDENTITY(1,1) NOT NULL,
    bpmn_code   VARCHAR(60)       DEFAULT '',
    tenant_id   VARCHAR(64)                DEFAULT '',
    is_del      TINYINT           NOT NULL DEFAULT 0,
    create_user VARCHAR(50)                DEFAULT '',
    create_time DATETIME          NOT NULL DEFAULT GETDATE(),
    update_user VARCHAR(50)                DEFAULT '',
    update_time DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpmn_conf_notice_template PRIMARY KEY (id)
);

CREATE NONCLUSTERED INDEX index_bpmn_code
    ON t_bpmn_conf_notice_template (bpmn_code);


-- ----------------------------
-- Table structure for t_bpmn_view_page_button
-- ----------------------------
CREATE TABLE t_bpmn_view_page_button (
                                         id          INT IDENTITY(1,1) NOT NULL,
                                         conf_id     INT               NOT NULL,
                                         view_type   INT               NOT NULL,
                                         button_type INT               NOT NULL,
                                         button_name VARCHAR(60)       DEFAULT '',
                                         remark      VARCHAR(255)      DEFAULT '',
                                         tenant_id   VARCHAR(64)       DEFAULT '',
                                         is_del      TINYINT  NOT NULL DEFAULT 0,
                                         create_user VARCHAR(50)       DEFAULT '',
                                         create_time DATETIME NOT NULL DEFAULT GETDATE(),
                                         update_user VARCHAR(50)       DEFAULT '',
                                         update_time DATETIME          DEFAULT GETDATE(),
                                         CONSTRAINT PK_t_bpmn_view_page_button PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for t_bpmn_template
-- ----------------------------
CREATE TABLE t_bpmn_template
(
    id                INT IDENTITY(1,1) NOT NULL,
    conf_id           BIGINT,
    node_id           BIGINT,
    event             INT,
    informs           VARCHAR(255),
    emps              VARCHAR(255),
    roles             VARCHAR(255),
    funcs             VARCHAR(255),
    template_id       BIGINT,
    form_code         VARCHAR(50),
    message_send_type VARCHAR(50),
    is_del            TINYINT NOT NULL DEFAULT 0,
    tenant_id         VARCHAR(64)      DEFAULT '',
    create_time       DATETIME         DEFAULT GETDATE(),
    create_user       VARCHAR(50)      DEFAULT '',
    update_time       DATETIME         DEFAULT GETDATE(),
    update_user       VARCHAR(50)      DEFAULT '',
    CONSTRAINT PK_t_bpmn_template PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for t_information_template
-- ----------------------------
CREATE TABLE t_information_template
(
    id           BIGINT IDENTITY(1,1) NOT NULL,
    name         VARCHAR(30)       DEFAULT '',
    num              VARCHAR(10)          DEFAULT '',
    system_title VARCHAR(100)      DEFAULT '',
    system_content   VARCHAR(500)         DEFAULT '',
    mail_title   VARCHAR(100)      DEFAULT '',
    mail_content     VARCHAR(500)         DEFAULT '',
    note_content VARCHAR(200)      DEFAULT '',
    jump_url         INT                  ,
    remark           VARCHAR(200)         DEFAULT '',
    status       TINYINT  NOT NULL DEFAULT 0,
    event        INT,
    event_name   VARCHAR(50),
    is_del       TINYINT  NOT NULL DEFAULT 0,
    tenant_id    VARCHAR(64)       DEFAULT '',
    create_time  DATETIME NOT NULL DEFAULT GETDATE(),
    create_user  VARCHAR(50)       DEFAULT '',
    update_time  DATETIME          DEFAULT GETDATE(),
    update_user  VARCHAR(50)       DEFAULT '',
    CONSTRAINT PK_t_information_template PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for bpm_business
-- ----------------------------
CREATE TABLE bpm_business
(
    id               BIGINT IDENTITY(1,1) NOT NULL,
    business_id      NVARCHAR(64),
    create_time      DATETIME NOT NULL DEFAULT GETDATE(),
    process_code     NVARCHAR(50),
    create_user_name NVARCHAR(50),
    create_user      NVARCHAR(50),
    process_key      NVARCHAR(50),
    tenant_id        NVARCHAR(64)         DEFAULT '',
    is_del           INT               DEFAULT 0,
    CONSTRAINT PK_bpm_business PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for bpm_flowrun_entrust
-- ----------------------------
CREATE TABLE bpm_flowrun_entrust
(
    id            INT IDENTITY(1,1) NOT NULL,
    runinfoid     NVARCHAR(64),
    runtaskid     NVARCHAR(64),
    original      NVARCHAR(64),
    original_name NVARCHAR(255),
    actual        NVARCHAR(64),
    actual_name   NVARCHAR(100),
    type          INT,
    is_read       INT          DEFAULT 2,
    proc_def_id   NVARCHAR(100),
    is_view       INT NOT NULL DEFAULT 0,
    tenant_id     NVARCHAR(64)      DEFAULT '',
    is_del        INT          DEFAULT 0,
    node_id       NVARCHAR(100),
    action_type   INT         default 0,
    CONSTRAINT PK_bpm_flowrun_entrust PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX BPM_IDX_ID
    ON bpm_flowrun_entrust (runinfoid, original, actual);

-- ----------------------------
-- Table structure for bpm_flowruninfo
-- ----------------------------
CREATE TABLE bpm_flowruninfo
(
    id            BIGINT IDENTITY(1,1) NOT NULL,
    runinfoid     NVARCHAR(64)         NOT NULL,
    create_UserId NVARCHAR(64),
    entitykey     NVARCHAR(100),
    entityclass   NVARCHAR(100),
    entitykeytype NVARCHAR(10),
    createactor   NVARCHAR(50),
    createdepart  NVARCHAR(100),
    createdate    DATETIME NOT NULL DEFAULT GETDATE(),
    tenant_id     NVARCHAR(64)         DEFAULT '',
    is_del        INT               DEFAULT 0,
    CONSTRAINT PK_bpm_flowruninfo PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for bpm_manual_notify
-- ----------------------------
CREATE TABLE bpm_manual_notify
(
    id          BIGINT IDENTITY(1,1) NOT NULL,
    business_id BIGINT   NOT NULL,
    code        NVARCHAR(10)      NOT NULL,
    last_time   DATETIME NOT NULL DEFAULT GETDATE(),
    create_time DATETIME NOT NULL DEFAULT GETDATE(),
    update_time DATETIME          DEFAULT GETDATE(),
    tenant_id   NVARCHAR(64)      DEFAULT '',
    is_del      INT               DEFAULT 0,
    CONSTRAINT PK_bpm_manual_notify PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for t_bpmn_approve_remind
-- ----------------------------
CREATE TABLE t_bpmn_approve_remind
(
    id          BIGINT IDENTITY(1,1) NOT NULL,
    conf_id     BIGINT,
    node_id     BIGINT,
    template_id BIGINT,
    days        NVARCHAR(255),
    is_del      TINYINT NOT NULL DEFAULT 0,
    tenant_id   NVARCHAR(64)         DEFAULT '',
    create_time DATETIME         DEFAULT GETDATE(),
    create_user NVARCHAR(50)         DEFAULT '',
    update_time DATETIME         DEFAULT GETDATE(),
    update_user NVARCHAR(50)         DEFAULT '',
    CONSTRAINT PK_t_bpmn_approve_remind PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for t_bpmn_conf_notice_template_detail
-- ----------------------------
CREATE TABLE t_bpmn_conf_notice_template_detail
(
    id                     BIGINT IDENTITY(1,1) NOT NULL,
    bpmn_code              NVARCHAR(60)         DEFAULT '',
    notice_template_type   INT      NOT NULL DEFAULT 1,
    notice_template_detail NVARCHAR(512),
    is_del                 TINYINT  NOT NULL DEFAULT 0,
    tenant_id              NVARCHAR(64)         DEFAULT '',
    create_user            NVARCHAR(50)         DEFAULT '',
    create_time            DATETIME NOT NULL DEFAULT GETDATE(),
    update_user            NVARCHAR(50)         DEFAULT '',
    update_time            DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpmn_conf_notice_template_detail PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX index_bpmn_code
    ON t_bpmn_conf_notice_template_detail (bpmn_code);

CREATE
NONCLUSTERED INDEX index_bpmn_type
    ON t_bpmn_conf_notice_template_detail (notice_template_type);

-- ----------------------------
-- Table structure for t_bpmn_node_conditions_conf
-- ----------------------------
CREATE TABLE t_bpmn_node_conditions_conf
(
    id             BIGINT IDENTITY(1,1) NOT NULL,
    bpmn_node_id   BIGINT   NOT NULL,
    is_default     INT      NOT NULL DEFAULT 0,
    sort           INT      NOT NULL,
    group_relation TINYINT,
    ext_json       NVARCHAR(2000),
    remark         NVARCHAR(255)        DEFAULT '',
    is_del         TINYINT  NOT NULL DEFAULT 0,
    tenant_id      NVARCHAR(64)         DEFAULT '',
    create_user    NVARCHAR(50)         DEFAULT '',
    create_time    DATETIME NOT NULL DEFAULT GETDATE(),
    update_user    NVARCHAR(50)         DEFAULT '',
    update_time    DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpmn_node_conditions_conf PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for t_bpmn_node_conditions_param_conf
-- ----------------------------
CREATE TABLE t_bpmn_node_conditions_param_conf
(
    id                      BIGINT IDENTITY(1,1) NOT NULL,
    bpmn_node_conditions_id BIGINT   NOT NULL,
    condition_param_type    INT      NOT NULL,
    condition_param_name    NVARCHAR(50)         NOT NULL,
    condition_param_jsom    NVARCHAR(MAX)        NOT NULL,
    operator                INT,
    cond_relation           TINYINT,
    cond_group              INT,
    remark                  NVARCHAR(255)        DEFAULT '',
    is_del                  TINYINT  NOT NULL DEFAULT 0,
    tenant_id               NVARCHAR(64)         DEFAULT '',
    create_user             NVARCHAR(50)         DEFAULT '',
    create_time             DATETIME NOT NULL DEFAULT GETDATE(),
    update_user             NVARCHAR(50)         DEFAULT '',
    update_time             DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpmn_node_conditions_param_conf PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for t_bpmn_node_sign_up_conf
-- ----------------------------
CREATE TABLE t_bpmn_node_sign_up_conf
(
    id                BIGINT IDENTITY(1,1) NOT NULL,
    bpmn_node_id      BIGINT   NOT NULL,
    after_sign_up_way INT      NOT NULL DEFAULT 1,
    sign_up_type      INT      NOT NULL DEFAULT 1,
    remark            NVARCHAR(255)        DEFAULT '',
    is_del            TINYINT  NOT NULL DEFAULT 0,
    tenant_id         NVARCHAR(64)         DEFAULT '',
    create_user       NVARCHAR(50)         DEFAULT '',
    create_time       DATETIME NOT NULL DEFAULT GETDATE(),
    update_user       NVARCHAR(50)         DEFAULT '',
    update_time       DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpmn_node_sign_up_conf PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for t_bpmn_node_to
-- ----------------------------
CREATE TABLE t_bpmn_node_to
(
    id           BIGINT IDENTITY(1,1) NOT NULL,
    bpmn_node_id BIGINT   NOT NULL,
    node_to      NVARCHAR(60)         DEFAULT '',
    remark         NVARCHAR(255)        DEFAULT '',
    is_del       TINYINT  NOT NULL DEFAULT 0,
    tenant_id    NVARCHAR(64)         DEFAULT '',
    create_user  NVARCHAR(50)         DEFAULT '',
    create_time  DATETIME NOT NULL DEFAULT GETDATE(),
    update_user  NVARCHAR(50)         DEFAULT '',
    update_time  DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpmn_node_to PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for bpm_process_dept
-- ----------------------------
CREATE TABLE bpm_process_dept
(
    id           BIGINT IDENTITY(1,1) NOT NULL,
    process_code NVARCHAR(50),
    process_type INT,
    process_name NVARCHAR(50),
    dep_id       BIGINT,
    remarks      NVARCHAR(255),
    create_time  DATETIME NOT NULL DEFAULT GETDATE(),
    create_user  BIGINT,
    update_user  BIGINT,
    update_time  DATETIME          DEFAULT GETDATE(),
    process_key  NVARCHAR(50),
    is_del       TINYINT           DEFAULT 0,
    tenant_id    NVARCHAR(64)         DEFAULT '',
    is_all       TINYINT           DEFAULT 0,
    CONSTRAINT PK_bpmn_process_dept PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for bpm_process_forward
-- ----------------------------
CREATE TABLE bpm_process_forward
(
    id                 BIGINT IDENTITY(1,1) NOT NULL,
    forward_user_id    NVARCHAR(50),
    Forward_user_name  NVARCHAR(50),
    processInstance_Id NVARCHAR(64),
    node_id            NVARCHAR(64),
    create_time        DATETIME NOT NULL DEFAULT GETDATE(),
    create_user_id     NVARCHAR(50),
    task_id            NVARCHAR(50),
    is_read            INT               DEFAULT 0,
    is_del             INT               DEFAULT 0,
    tenant_id          NVARCHAR(64)         DEFAULT '',
    update_time        DATETIME          DEFAULT GETDATE(),
    process_number     NVARCHAR(50)         DEFAULT '',
    CONSTRAINT PK_bpm_process_forward PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX forward_user_id
    ON bpm_process_forward (forward_user_id);

CREATE
NONCLUSTERED INDEX index_forward_user_id_is_read
    ON bpm_process_forward (forward_user_id, is_read);


-- ----------------------------
-- Table structure for bpm_process_node_overtime
-- ----------------------------
CREATE TABLE bpm_process_node_overtime
(
    id          BIGINT IDENTITY(1,1) NOT NULL,
    notice_type INT,
    node_name   NVARCHAR(50),
    node_key    NVARCHAR(50),
    process_key NVARCHAR(50),
    notice_time INT,
    is_del      INT DEFAULT 0,
    tenant_id   NVARCHAR(64)         DEFAULT '',
    CONSTRAINT PK_bpm_process_node_overtime PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for bpm_process_node_record
-- ----------------------------
CREATE TABLE bpm_process_node_record
(
    id                 BIGINT IDENTITY(1,1) NOT NULL,
    processInstance_id NVARCHAR(64),
    task_id            NVARCHAR(50),
    create_time        DATETIME NOT NULL DEFAULT GETDATE(),
    is_del             INT               DEFAULT 0,
    tenant_id          NVARCHAR(64)         DEFAULT '',
    CONSTRAINT PK_bpm_process_node_record PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for bpm_process_node_submit
-- ----------------------------
CREATE TABLE bpm_process_node_submit
(
    id                 BIGINT IDENTITY(1,1) NOT NULL,
    processInstance_Id NVARCHAR(64),
    back_type          TINYINT,
    node_key           NVARCHAR(50),
    create_time        DATETIME NOT NULL DEFAULT GETDATE(),
    create_user        NVARCHAR(50),
    state              TINYINT,
    is_del             INT               DEFAULT 0,
    tenant_id          NVARCHAR(64)         DEFAULT '',
    CONSTRAINT PK_bpm_process_node_submit PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for bpm_process_notice
-- ----------------------------
CREATE TABLE bpm_process_notice
(
    id          BIGINT IDENTITY(1,1) NOT NULL,
    type        TINYINT,
    process_key NVARCHAR(50),
    is_del      INT DEFAULT 0,
    tenant_id   NVARCHAR(64)         DEFAULT '',
    CONSTRAINT PK_bpm_process_notice PRIMARY KEY (id),
    CONSTRAINT UK_bpm_process_notice_process_key_type UNIQUE (process_key, type)
);

-- ----------------------------
-- Table structure for bpm_taskconfig
-- ----------------------------
CREATE TABLE bpm_taskconfig
(
    id            BIGINT IDENTITY(1,1) NOT NULL,
    proc_def_id_  NVARCHAR(100),
    task_def_key_ NVARCHAR(100),
    user_id       BIGINT,
    number        INT,
    status        TINYINT,
    original_type TINYINT,
    is_del        INT DEFAULT 0,
    tenant_id     NVARCHAR(64)         DEFAULT '',
    CONSTRAINT PK_bpm_taskconfig PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX BPM_IDX__TASK_CONFIG
    ON bpm_taskconfig (proc_def_id_, task_def_key_);

-- ----------------------------
-- Table structure for t_bpm_variable
-- ----------------------------
CREATE TABLE t_bpm_variable
(
    id           BIGINT IDENTITY(1,1) NOT NULL,
    process_num  NVARCHAR(60)         DEFAULT '',
    process_name             NVARCHAR(60)         DEFAULT '',
    process_desc NVARCHAR(255)        DEFAULT '',
    process_start_conditions NVARCHAR(MAX)        NOT NULL,
    bpmn_code                NVARCHAR(60)         DEFAULT '',
    is_new_data              INT                  DEFAULT 0,
    remark                   NVARCHAR(255)        DEFAULT '',
    is_del       TINYINT  NOT NULL DEFAULT 0,
    tenant_id    NVARCHAR(64)         DEFAULT '',
    create_user  NVARCHAR(50)         DEFAULT '',
    create_time  DATETIME NOT NULL DEFAULT GETDATE(),
    update_user  NVARCHAR(50)         DEFAULT '',
    update_time  DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpm_variable PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX index_process_num
    ON t_bpm_variable (process_num);


-- ----------------------------
-- Table structure for t_bpm_variable_approve_remind
-- ----------------------------
CREATE TABLE t_bpm_variable_approve_remind
(
    id   BIGINT IDENTITY(1,1) NOT NULL,
    variable_id BIGINT   NOT NULL,
    element_id  NVARCHAR(60)         DEFAULT '',
    content      NVARCHAR(MAX)        NOT NULL,
    remark       NVARCHAR(255)        DEFAULT '',
    is_del      TINYINT  NOT NULL DEFAULT 0,
    tenant_id   NVARCHAR(64)         DEFAULT '',
    create_user NVARCHAR(50)         DEFAULT '',
    create_time DATETIME NOT NULL DEFAULT GETDATE(),
    update_user NVARCHAR(50)         DEFAULT '',
    update_time DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpm_variable_approve_remind PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX variable_id_element_id
    ON t_bpm_variable_approve_remind (variable_id, element_id);


-- ----------------------------
-- Table structure for t_bpm_variable_button
-- ----------------------------
CREATE TABLE t_bpm_variable_button
(
    id               BIGINT IDENTITY(1,1) NOT NULL,
    variable_id      BIGINT   NOT NULL,
    element_id       NVARCHAR(60)         DEFAULT '',
    button_page_type INT      NOT NULL,
    button_type      INT      NOT NULL,
    button_name      NVARCHAR(60)         DEFAULT '',
    remark           NVARCHAR(255)        DEFAULT '',
    is_del           TINYINT  NOT NULL DEFAULT 0,
    tenant_id        NVARCHAR(64)         DEFAULT '',
    create_user      NVARCHAR(50)         DEFAULT '',
    create_time      DATETIME NOT NULL DEFAULT GETDATE(),
    update_user      NVARCHAR(50)         DEFAULT '',
    update_time      DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpm_variable_button PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX variable_id
    ON t_bpm_variable_button (variable_id);


-- ----------------------------
-- Table structure for t_bpm_variable_message
-- ----------------------------
CREATE TABLE t_bpm_variable_message
(
    id           BIGINT IDENTITY(1,1) NOT NULL,
    variable_id  BIGINT   NOT NULL,
    element_id   NVARCHAR(60)         DEFAULT '',
    message_type INT      NOT NULL DEFAULT 0,
    event_type   INT      NOT NULL DEFAULT 0,
    content      NVARCHAR(MAX)        NOT NULL,
    remark       NVARCHAR(255)        DEFAULT '',
    is_del       TINYINT  NOT NULL DEFAULT 0,
    tenant_id    NVARCHAR(64)         DEFAULT '',
    create_user  NVARCHAR(50)         DEFAULT '',
    create_time  DATETIME NOT NULL DEFAULT GETDATE(),
    update_user  NVARCHAR(50)         DEFAULT '',
    update_time  DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpm_variable_message PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX variable_id_element_id_message_type_event_type
    ON t_bpm_variable_message (variable_id, element_id, message_type, event_type);

CREATE
NONCLUSTERED INDEX variable_id_message_type_event_type
    ON t_bpm_variable_message (variable_id, message_type, event_type);

-- ----------------------------
-- Table structure for t_bpm_variable_multiplayer
-- ----------------------------
CREATE TABLE t_bpm_variable_multiplayer
(
    id              BIGINT IDENTITY(1,1) NOT NULL,
    variable_id     BIGINT   NOT NULL,
    element_id      NVARCHAR(60)         DEFAULT '',
    element_name    NVARCHAR(60)         DEFAULT '',
    node_id         NVARCHAR(60),
    collection_name NVARCHAR(60)         DEFAULT '',
    sign_type       INT      NOT NULL,
    remark          NVARCHAR(255)        DEFAULT '',
    is_del          TINYINT  NOT NULL DEFAULT 0,
    tenant_id       NVARCHAR(64)         DEFAULT '',
    create_user     NVARCHAR(50)         DEFAULT '',
    create_time     DATETIME NOT NULL DEFAULT GETDATE(),
    update_user     NVARCHAR(50)         DEFAULT '',
    update_time     DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpm_variable_multiplayer PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX variable_id
    ON t_bpm_variable_multiplayer (variable_id);

CREATE
NONCLUSTERED INDEX variable_id_element_id
    ON t_bpm_variable_multiplayer (variable_id, element_id);

-- ----------------------------
-- Table structure for t_bpm_variable_multiplayer_personnel
-- ----------------------------
CREATE TABLE t_bpm_variable_multiplayer_personnel
(
    id                      BIGINT IDENTITY(1,1) NOT NULL,
    variable_multiplayer_id BIGINT   NOT NULL,
    assignee                NVARCHAR(60)         DEFAULT '',
    assignee_name           NVARCHAR(60)         DEFAULT '',
    undertake_status        INT      NOT NULL,
    remark                  NVARCHAR(255)        DEFAULT '',
    is_del                  TINYINT  NOT NULL DEFAULT 0,
    tenant_id               NVARCHAR(64)         DEFAULT '',
    create_user             NVARCHAR(50)         DEFAULT '',
    create_time             DATETIME NOT NULL DEFAULT GETDATE(),
    update_user             NVARCHAR(50)         DEFAULT '',
    update_time             DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpm_variable_multiplayer_personnel PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX variable_multiplayer_id
    ON t_bpm_variable_multiplayer_personnel (variable_multiplayer_id);

-- ----------------------------
-- Table structure for t_bpm_variable_sequence_flow
-- ----------------------------
CREATE TABLE t_bpm_variable_sequence_flow
(
    id                       BIGINT IDENTITY(1,1) NOT NULL,
    variable_id              BIGINT   NOT NULL,
    element_id               NVARCHAR(60)         DEFAULT '',
    element_name             NVARCHAR(60)         DEFAULT '',
    element_from_id          NVARCHAR(60)         DEFAULT '',
    element_to_id            NVARCHAR(60)         DEFAULT '',
    sequence_flow_type       INT      NOT NULL,
    sequence_flow_conditions NVARCHAR(100)        DEFAULT '',
    remark                   NVARCHAR(255)        DEFAULT '',
    is_del                   TINYINT  NOT NULL DEFAULT 0,
    tenant_id                NVARCHAR(64)         DEFAULT '',
    create_user              NVARCHAR(50)         DEFAULT '',
    create_time              DATETIME NOT NULL DEFAULT GETDATE(),
    update_user              NVARCHAR(50)         DEFAULT '',
    update_time              DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpm_variable_sequence_flow PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for t_bpm_variable_sign_up
-- ----------------------------
CREATE TABLE t_bpm_variable_sign_up
(
    id                BIGINT IDENTITY(1,1) NOT NULL,
    variable_id       BIGINT   NOT NULL,
    element_id        NVARCHAR(60)         DEFAULT '',
    node_id           NVARCHAR(60),
    after_sign_up_way INT      NOT NULL DEFAULT 1,
    sub_elements      NVARCHAR(MAX)        NOT NULL,
    remark            NVARCHAR(255)        DEFAULT '',
    is_del            TINYINT  NOT NULL DEFAULT 0,
    tenant_id         NVARCHAR(64)         DEFAULT '',
    create_user       NVARCHAR(50)         DEFAULT '',
    create_time       DATETIME NOT NULL DEFAULT GETDATE(),
    update_user       NVARCHAR(50)         DEFAULT '',
    update_time       DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpm_variable_sign_up PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX variable_id
    ON t_bpm_variable_sign_up (variable_id);

CREATE
NONCLUSTERED INDEX variable_id_element_id
    ON t_bpm_variable_sign_up (variable_id, element_id);


-- ----------------------------
-- Table structure for t_bpm_variable_sign_up_personnel
-- ----------------------------
CREATE TABLE t_bpm_variable_sign_up_personnel
(
    id            BIGINT IDENTITY(1,1) NOT NULL,
    variable_id   BIGINT   NOT NULL,
    element_id    NVARCHAR(60)         DEFAULT '',
    assignee      NVARCHAR(60)         DEFAULT '',
    assignee_name NVARCHAR(60)         DEFAULT '',
    remark        NVARCHAR(255)        DEFAULT '',
    is_del        TINYINT  NOT NULL DEFAULT 0,
    tenant_id     NVARCHAR(64)         DEFAULT '',
    create_user   NVARCHAR(50)         DEFAULT '',
    create_time   DATETIME NOT NULL DEFAULT GETDATE(),
    update_user   NVARCHAR(50)         DEFAULT '',
    update_time   DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpm_variable_sign_up_personnel PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX variable_id
    ON t_bpm_variable_sign_up_personnel (variable_id);

CREATE
NONCLUSTERED INDEX variable_id_element_id
    ON t_bpm_variable_sign_up_personnel (variable_id, element_id);


-- ----------------------------
-- Table structure for t_bpm_variable_single
-- ----------------------------
CREATE TABLE t_bpm_variable_single
(
    id                  BIGINT IDENTITY(1,1) NOT NULL,
    variable_id         BIGINT   NOT NULL,
    element_id          NVARCHAR(60)         DEFAULT '',
    node_id             NVARCHAR(60),
    element_name        NVARCHAR(60)         DEFAULT '',
    assignee_param_name NVARCHAR(60)         DEFAULT '',
    assignee            NVARCHAR(60)         DEFAULT '',
    assignee_name       NVARCHAR(60)         DEFAULT '',
    remark              NVARCHAR(255)        DEFAULT '',
    is_del              TINYINT  NOT NULL DEFAULT 0,
    tenant_id           NVARCHAR(64)         DEFAULT '',
    create_user         NVARCHAR(50)         DEFAULT '',
    create_time         DATETIME NOT NULL DEFAULT GETDATE(),
    update_user         NVARCHAR(50)         DEFAULT '',
    update_time         DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpm_variable_single PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX variable_id
    ON t_bpm_variable_single (variable_id);

CREATE
NONCLUSTERED INDEX variable_id_element_id
    ON t_bpm_variable_single (variable_id, element_id);


-- ----------------------------
-- Table structure for t_bpm_variable_view_page_button
-- ----------------------------
CREATE TABLE t_bpm_variable_view_page_button
(
    id          BIGINT IDENTITY(1,1) NOT NULL,
    variable_id BIGINT   NOT NULL,
    view_type   INT      NOT NULL,
    button_type INT      NOT NULL,
    button_name NVARCHAR(60)         DEFAULT '',
    remark        NVARCHAR(255)        DEFAULT '',
    is_del      TINYINT  NOT NULL DEFAULT 0,
    tenant_id   NVARCHAR(64)         DEFAULT '',
    create_user NVARCHAR(50)         DEFAULT '',
    create_time DATETIME NOT NULL DEFAULT GETDATE(),
    update_user NVARCHAR(50)         DEFAULT '',
    update_time DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpm_variable_view_page_button PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX index_variable_id
    ON t_bpm_variable_view_page_button (variable_id);

-- ----------------------------
-- Table structure for bpm_verify_info
-- ----------------------------
CREATE TABLE bpm_verify_info
(
    id               BIGINT IDENTITY(1,1) NOT NULL,
    run_info_id      NVARCHAR(64),
    verify_user_id   NVARCHAR(50),
    verify_user_name NVARCHAR(100),
    verify_status    INT,
    verify_desc      NVARCHAR(500),
    verify_date      DATETIME NOT NULL DEFAULT GETDATE(),
    task_name        NVARCHAR(64),
    task_id          NVARCHAR(64),
    task_def_key     NVARCHAR(255),
    business_type    INT,
    business_id      NVARCHAR(128),
    original_id      NVARCHAR(64),
    process_code     NVARCHAR(64),
    is_del           TINYINT  NOT NULL DEFAULT 0,
    tenant_id        NVARCHAR(64)         DEFAULT '',
    CONSTRAINT PK_bpm_verify_info PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX BPM_IDX__INFOR
    ON bpm_verify_info (business_type, business_id);

CREATE
NONCLUSTERED INDEX process_code_index
    ON bpm_verify_info (process_code);

-- ----------------------------
-- Table structure for t_default_template
-- ----------------------------
CREATE TABLE t_default_template
(
    id          BIGINT IDENTITY(1,1) NOT NULL,
    event       INT,
    template_id BIGINT,
    is_del      TINYINT  NOT NULL DEFAULT 0,
    tenant_id   NVARCHAR(64)         DEFAULT '',
    create_time DATETIME NOT NULL DEFAULT GETDATE(),
    create_user NVARCHAR(255)        DEFAULT '',
    update_time DATETIME          DEFAULT GETDATE(),
    update_user NVARCHAR(255)        DEFAULT '',
    CONSTRAINT PK_t_default_template PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for t_user_email_send
-- ----------------------------
CREATE TABLE t_user_email_send
(
    id          INT IDENTITY(1,1) NOT NULL,
    sender      NVARCHAR(32)      NOT NULL,
    receiver    NVARCHAR(100)     NOT NULL,
    title       NVARCHAR(255)     NOT NULL,
    content     NVARCHAR(MAX)     NOT NULL,
    create_time DATETIME NOT NULL DEFAULT GETDATE(),
    update_time DATETIME          DEFAULT GETDATE(),
    create_user NVARCHAR(50)      NOT NULL,
    update_user NVARCHAR(50)      NOT NULL,
    is_del      TINYINT  NOT NULL DEFAULT 0,
    tenant_id   NVARCHAR(64)      DEFAULT '',
    CONSTRAINT PK_t_user_email_send PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX sender
    ON t_user_email_send (receiver);

-- ----------------------------
-- Table structure for t_method_replay
-- ----------------------------
CREATE TABLE t_method_replay
(
    id                   INT IDENTITY(1,1) NOT NULL,
    PROJECT_NAME         NVARCHAR(100),
    CLASS_NAME           NVARCHAR(255),
    METHOD_NAME          NVARCHAR(255),
    PARAM_TYPE           NVARCHAR(255),
    ARGS                 NVARCHAR(MAX),
    NOW_TIME             DATETIME,
    ERROR_MSG            NVARCHAR(MAX),
    ALREADY_REPLAY_TIMES INT,
    MAX_REPLAY_TIMES     INT,
    CONSTRAINT PK_t_method_replay PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX t_method_replay_NOW_TIME_index
    ON t_method_replay (NOW_TIME);

-- ----------------------------
-- Table structure for t_user_entrust
-- ----------------------------
CREATE TABLE t_user_entrust
(
    id            INT IDENTITY(1,1) NOT NULL,
    sender        NVARCHAR(64)      NOT NULL,
    receiver_id   NVARCHAR(64)      NOT NULL,
    receiver_name NVARCHAR(255),
    power_id      NVARCHAR(100)     NOT NULL,
    begin_time    DATETIME,
    end_time      DATETIME,
    create_time   DATETIME NOT NULL DEFAULT GETDATE(),
    update_time   DATETIME          DEFAULT GETDATE(),
    create_user   NVARCHAR(50)      NOT NULL,
    update_user   NVARCHAR(50)      NOT NULL,
    is_del        TINYINT  NOT NULL DEFAULT 0,
    tenant_id     NVARCHAR(64)      DEFAULT '',
    CONSTRAINT PK_t_user_entrust PRIMARY KEY (id)
);

-- 唯一约束：sender + receiver_id + power_id
CREATE
UNIQUE
NONCLUSTERED INDEX s_r_id
    ON t_user_entrust (sender, receiver_id, power_id);

-- 普通索引：sender + power_id
CREATE
NONCLUSTERED INDEX user_id
    ON t_user_entrust (sender, power_id);

-- ----------------------------
-- Table structure for t_user_message_status
-- ----------------------------
CREATE TABLE t_user_message_status
(
    id                     INT IDENTITY(1,1) NOT NULL,
    user_id                NVARCHAR(64)      NOT NULL,
    message_status         TINYINT  NOT NULL DEFAULT 0,
    mail_status            TINYINT  NOT NULL DEFAULT 0,
    not_trouble_time_end   TIME,
    not_trouble_time_begin DATETIME,
    not_trouble            TINYINT  NOT NULL DEFAULT 0,
    shock                  TINYINT  NOT NULL DEFAULT 0,
    sound                  TINYINT  NOT NULL DEFAULT 0,
    open_phone             TINYINT  NOT NULL DEFAULT 0,
    create_time            DATETIME NOT NULL DEFAULT GETDATE(),
    update_time            DATETIME          DEFAULT GETDATE(),
    create_user            NVARCHAR(50)      NOT NULL,
    update_user            NVARCHAR(50)      NOT NULL,
    is_del                 TINYINT  NOT NULL DEFAULT 0,
    tenant_id              NVARCHAR(64)      DEFAULT '',
    CONSTRAINT PK_t_user_message_status PRIMARY KEY (id)
);


CREATE
NONCLUSTERED INDEX user_id
    ON t_user_message_status (user_id);

-- ----------------------------
-- Table structure for t_bpmn_node_button_conf
-- ----------------------------
CREATE TABLE t_bpmn_node_button_conf
(
    id               BIGINT IDENTITY(1,1) NOT NULL,
    bpmn_node_id     BIGINT   NOT NULL,
    button_page_type INT      NOT NULL,
    button_type      INT      NOT NULL,
    button_name      NVARCHAR(60)         DEFAULT '',
    remark           NVARCHAR(255)        DEFAULT '',
    is_del           TINYINT  NOT NULL DEFAULT 0,
    tenant_id        NVARCHAR(64)         DEFAULT '',
    create_user      NVARCHAR(50)         DEFAULT '',
    create_time      DATETIME NOT NULL DEFAULT GETDATE(),
    update_user      NVARCHAR(50)         DEFAULT '',
    update_time      DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpmn_node_button_conf PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for bpm_business_process
-- ----------------------------
CREATE TABLE bpm_business_process
(
    id                  BIGINT IDENTITY(1,1) NOT NULL,
    PROCESSINESS_KEY    NVARCHAR(64),
    BUSINESS_ID         NVARCHAR(64)         NOT NULL,
    BUSINESS_NUMBER     NVARCHAR(64),
    ENTRY_ID            NVARCHAR(64),
    VERSION             NVARCHAR(30),
    CREATE_TIME         DATETIME NOT NULL DEFAULT GETDATE(),
    UPDATE_TIME         DATETIME          DEFAULT GETDATE(),
    description         NVARCHAR(100),
    process_state       INT,
    create_user         NVARCHAR(64),
    process_digest      NVARCHAR(MAX),
    is_del              TINYINT           DEFAULT 0,
    tenant_id           NVARCHAR(64)         DEFAULT '',
    data_source_id      BIGINT,
    PROC_INST_ID_       NVARCHAR(64)         DEFAULT '',
    back_user_id        NVARCHAR(64),
    user_name           NVARCHAR(255),
    is_out_side_process TINYINT           DEFAULT 0,
    is_lowcode_flow     TINYINT           DEFAULT 0,
    CONSTRAINT PK_bpm_business_process PRIMARY KEY (id)
);


CREATE
NONCLUSTERED INDEX PROC_INST_ID_index
    ON bpm_business_process (PROC_INST_ID_);

CREATE
NONCLUSTERED INDEX process_entry_id
    ON bpm_business_process (ENTRY_ID);

CREATE
NONCLUSTERED INDEX process_key_index
    ON bpm_business_process (PROCESSINESS_KEY);

CREATE
NONCLUSTERED INDEX process_number_index
    ON bpm_business_process (BUSINESS_NUMBER);

CREATE
NONCLUSTERED INDEX process_state_index
    ON bpm_business_process (process_state);


-- ----------------------------
-- Table structure for bpm_process_name_relevancy
-- ----------------------------
CREATE TABLE bpm_process_name_relevancy
(
    id              BIGINT IDENTITY(1,1) NOT NULL,
    process_name_id BIGINT,
    process_key     NVARCHAR(50),
    is_del          INT               DEFAULT 0,
    tenant_id       NVARCHAR(64)         DEFAULT '',
    create_time     DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_bpm_process_name_relevancy PRIMARY KEY (id)
);



CREATE
NONCLUSTERED INDEX process_key_index
    ON bpm_process_name_relevancy (process_key);

CREATE
NONCLUSTERED INDEX process_name_id_index
    ON bpm_process_name_relevancy (process_name_id);


-- ----------------------------
-- Table structure for t_bpmn_node_personnel_conf
-- ----------------------------
CREATE TABLE t_bpmn_node_personnel_conf
(
    id           INT IDENTITY(1,1) NOT NULL,
    bpmn_node_id INT      NOT NULL,
    sign_type    TINYINT,
    remark       NVARCHAR(100),
    is_del       TINYINT,
    tenant_id    NVARCHAR(64)      DEFAULT '',
    create_user  NVARCHAR(50),
    create_time  DATETIME NOT NULL DEFAULT GETDATE(),
    update_user  NVARCHAR(50),
    update_time  DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpmn_node_personnel_conf PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for t_bpmn_node_personnel_empl_conf
-- ----------------------------
CREATE TABLE t_bpmn_node_personnel_empl_conf
(
    id                    INT IDENTITY(1,1) NOT NULL,
    bpmn_node_personne_id INT      NOT NULL,
    empl_id               NVARCHAR(50)      NOT NULL,
    empl_name             NVARCHAR(50),
    remark                NVARCHAR(100),
    is_del                TINYINT,
    tenant_id             NVARCHAR(64)      DEFAULT '',
    create_user           NVARCHAR(30),
    create_time           DATETIME NOT NULL DEFAULT GETDATE(),
    update_user           NVARCHAR(30),
    update_time           DATETIME          DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpmn_node_personnel_empl_conf PRIMARY KEY (id)
);



-- ----------------------------
-- Table structure for bpm_process_operation
-- ----------------------------
CREATE TABLE bpm_process_operation
(
    id           BIGINT IDENTITY(1,1) NOT NULL,
    process_key  NVARCHAR(50),
    process_node NVARCHAR(50),
    type         INT,
    is_del       TINYINT,
    tenant_id    NVARCHAR(64) DEFAULT '',
    CONSTRAINT PK_bpm_process_operation PRIMARY KEY (id)
);



-- ----------------------------
-- Table structure for bpm_process_node_back
-- ----------------------------
CREATE TABLE bpm_process_node_back
(
    id          BIGINT IDENTITY(1,1) NOT NULL,
    node_key    NVARCHAR(50),
    node_id     BIGINT,
    back_type   INT,
    process_key NVARCHAR(100),
    is_del      TINYINT,
    tenant_id   NVARCHAR(64) DEFAULT '',
    CONSTRAINT PK_bpm_process_node_back PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for bpm_process_name
-- ----------------------------
CREATE TABLE bpm_process_name
(
    id           BIGINT IDENTITY(1,1) NOT NULL,
    process_name NVARCHAR(50),
    is_del       INT      DEFAULT 0,
    tenant_id    NVARCHAR(64) DEFAULT '',
    create_time  DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_bpm_process_name PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for t_user_message
-- ----------------------------
CREATE TABLE t_user_message
(
    id          BIGINT IDENTITY(1,1) NOT NULL,
    user_id     NVARCHAR(64),
    title       NVARCHAR(50),
    content     NVARCHAR(255),
    url         NVARCHAR(255),
    node        NVARCHAR(50),
    params      NVARCHAR(255),
    is_read     TINYINT,
    is_del      TINYINT,
    tenant_id   NVARCHAR(64) DEFAULT '',
    create_time DATETIME,
    update_time DATETIME,
    create_user NVARCHAR(50),
    update_user NVARCHAR(50),
    app_url     NVARCHAR(255),
    source      INT,
    CONSTRAINT PK_t_user_message PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for t_op_log
-- ----------------------------
CREATE TABLE t_op_log
(
    id             BIGINT IDENTITY(1,1) NOT NULL,
    msg_id         NVARCHAR(64),
    op_flag        TINYINT,
    op_user_no     NVARCHAR(50),
    op_user_name   NVARCHAR(50),
    op_method      NVARCHAR(255),
    op_time        DATETIME DEFAULT GETDATE(),
    op_use_time    BIGINT,
    op_param       NVARCHAR(MAX),
    op_result      NVARCHAR(MAX),
    system_type    TINYINT,
    app_version    NVARCHAR(50),
    hardware       NVARCHAR(50),
    system_version NVARCHAR(50),
    remark         NVARCHAR(255),
    is_del         INT      DEFAULT 0,
    tenant_id      NVARCHAR(64) DEFAULT '',
    CONSTRAINT PK_t_op_log PRIMARY KEY (id)
);



-- ----------------------------
-- Table structure for t_biz_account_apply
-- ----------------------------
CREATE TABLE t_biz_account_apply
(
    id                 INT IDENTITY(1,1) NOT NULL,
    account_type       TINYINT,
    account_owner_name NVARCHAR(50),
    remark             NVARCHAR(200),
    is_del             INT DEFAULT 0,
    tenant_id          NVARCHAR(64) DEFAULT '',
    CONSTRAINT PK_t_biz_account_apply PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for t_bpmn_node_out_side_access_conf
-- ----------------------------
CREATE TABLE t_bpmn_node_out_side_access_conf
(
    id           BIGINT IDENTITY(1,1) NOT NULL,
    bpmn_node_id BIGINT,
    node_mark    NVARCHAR(50),
    sign_type    INT,
    remark       NVARCHAR(255),
    is_del       INT      DEFAULT 0,
    create_user  NVARCHAR(50),
    create_time  DATETIME DEFAULT GETDATE(),
    update_user  NVARCHAR(50),
    update_time  DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpmn_node_out_side_access_conf PRIMARY KEY (id)
);



-- ----------------------------
-- Table structure for bpm_process_app_application
-- ----------------------------
CREATE TABLE bpm_process_app_application
(
    id               INT IDENTITY(1,1) NOT NULL,
    business_code    NVARCHAR(50),
    process_name     NVARCHAR(50),
    apply_type       INT,
    permissions_code NVARCHAR(50),
    pc_icon          NVARCHAR(500),
    effective_source NVARCHAR(500),
    is_son           INT,
    look_url         NVARCHAR(500),
    submit_url       NVARCHAR(500),
    condition_url    NVARCHAR(500),
    parent_id        INT,
    application_url  NVARCHAR(500),
    user_request_uri NVARCHAR(500),
    role_request_uri NVARCHAR(500),
    route            NVARCHAR(500),
    process_key      NVARCHAR(50),
    create_time      DATETIME DEFAULT GETDATE(),
    update_time      DATETIME DEFAULT GETDATE(),
    is_del           TINYINT  DEFAULT 0,
    create_user_id   NVARCHAR(64),
    update_user      NVARCHAR(255),
    is_all           TINYINT  DEFAULT 0,
    state            TINYINT  DEFAULT 1,
    sort             INT,
    source           NVARCHAR(255),
    CONSTRAINT PK_bpm_process_app_application PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for bpm_process_app_data
-- ----------------------------
CREATE TABLE bpm_process_app_data
(
    id             BIGINT IDENTITY(1,1) NOT NULL,
    process_key    NVARCHAR(50),
    process_name   NVARCHAR(50),
    state          INT,
    route          NVARCHAR(500),
    sort           INT,
    source         NVARCHAR(500),
    is_all         TINYINT,
    version_id     BIGINT,
    application_id BIGINT,
    type           INT,
    CONSTRAINT PK_bpm_process_app_data PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for bpm_process_application_type
-- ----------------------------
CREATE TABLE bpm_process_application_type
(
    id               BIGINT IDENTITY(1,1) NOT NULL,
    application_id   BIGINT,
    category_id      BIGINT,
    is_del           INT,
    tenant_id        NVARCHAR(64) DEFAULT '',
    sort             INT,
    state            INT,
    history_id       BIGINT,
    visble_state     INT,
    create_time      DATETIME DEFAULT GETDATE(),
    common_use_state INT,
    CONSTRAINT PK_bpm_process_application_type PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for bpm_process_category
-- ----------------------------
CREATE TABLE bpm_process_category
(
    id                BIGINT IDENTITY(1,1) NOT NULL,
    process_type_name NVARCHAR(255),
    is_del            TINYINT,
    tenant_id         NVARCHAR(64) DEFAULT '',
    state             INT,
    sort              INT,
    is_app            TINYINT,
    entrance          NVARCHAR(255),
    CONSTRAINT PK_bpm_process_category PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for bpm_process_permissions
-- ----------------------------
CREATE TABLE bpm_process_permissions
(
    id               BIGINT IDENTITY(1,1) NOT NULL,
    user_id          NVARCHAR(64),
    dep_id           BIGINT,
    permissions_type INT,
    create_user      NVARCHAR(64),
    create_time      DATETIME DEFAULT GETDATE(),
    process_key      NVARCHAR(50),
    office_id        BIGINT,
    is_del           INT      DEFAULT 0,
    tenant_id        NVARCHAR(64) DEFAULT ''
);

ALTER TABLE bpm_process_permissions
    ADD CONSTRAINT PK_bpm_process_permissions PRIMARY KEY (id);


-- ----------------------------
-- Table structure for t_out_side_bpm_access_business
-- ----------------------------
CREATE TABLE t_out_side_bpm_access_business
(
    id                BIGINT IDENTITY(1,1) NOT NULL,
    business_party_id BIGINT,
    bpmn_conf_id      BIGINT,
    form_code         NVARCHAR(50),
    process_number    NVARCHAR(50),
    form_data_pc      NVARCHAR(MAX),
    form_data_app     NVARCHAR(MAX),
    template_mark     NVARCHAR(50),
    start_username    NVARCHAR(50),
    remark            NVARCHAR(MAX),
    is_del            TINYINT  DEFAULT 0,
    create_user       NVARCHAR(50),
    create_time       DATETIME DEFAULT GETDATE(),
    update_user       NVARCHAR(50),
    update_time       DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_t_out_side_bpm_access_business PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for t_out_side_bpm_admin_personnel
-- ----------------------------
CREATE TABLE t_out_side_bpm_admin_personnel
(
    id                BIGINT IDENTITY(1,1) NOT NULL,
    business_party_id BIGINT,
    type              INT,
    employee_id       NVARCHAR(64),
    employee_name     NVARCHAR(64),
    remark            NVARCHAR(255),
    is_del            INT,
    create_user       NVARCHAR(50),
    create_time       DATETIME DEFAULT GETDATE(),
    update_user       NVARCHAR(50),
    update_time       DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_t_out_side_bpm_admin_personnel PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for t_out_side_bpm_business_party
-- ----------------------------
CREATE TABLE t_out_side_bpm_business_party
(
    id                  BIGINT IDENTITY(1,1) NOT NULL,
    business_party_mark NVARCHAR(50),
    name                NVARCHAR(255),
    type                TINYINT,
    remark              NVARCHAR(255),
    is_del              TINYINT  DEFAULT 0,
    create_user         NVARCHAR(50),
    create_time         DATETIME DEFAULT GETDATE(),
    update_user         NVARCHAR(50),
    update_time         DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_t_out_side_bpm_business_party PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for t_out_side_bpm_callback_url_conf
-- ----------------------------
CREATE TABLE t_out_side_bpm_callback_url_conf
(
    id                    BIGINT IDENTITY(1,1) NOT NULL,
    business_party_id     BIGINT,
    application_id        BIGINT,
    bpmn_conf_id          BIGINT,
    form_code             NVARCHAR(64),
    bpm_conf_callback_url NVARCHAR(500),
    bpm_flow_callback_url NVARCHAR(500),
    api_client_id         NVARCHAR(100),
    api_client_secret     NVARCHAR(100),
    status                TINYINT  DEFAULT 0,
    create_user           NVARCHAR(50),
    update_user           NVARCHAR(50),
    remark                NVARCHAR(50),
    is_del                TINYINT  DEFAULT 0,
    create_time           DATETIME DEFAULT GETDATE(),
    update_time           DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_t_out_side_bpm_callback_url_conf PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for t_out_side_bpm_approve_template
-- ----------------------------
CREATE TABLE t_out_side_bpm_approve_template
(
    id                BIGINT IDENTITY(1,1) NOT NULL,
    business_party_id BIGINT,
    application_id    INT,
    approve_type_id   INT,
    approve_type_name NVARCHAR(50),
    api_client_id     NVARCHAR(50),
    api_client_secret NVARCHAR(50),
    api_token         NVARCHAR(50),
    api_url           NVARCHAR(50),
    remark            NVARCHAR(255),
    is_del            TINYINT  DEFAULT 0,
    create_user       NVARCHAR(50),
    create_time       DATETIME DEFAULT GETDATE(),
    update_user       NVARCHAR(50),
    update_time       DATETIME DEFAULT GETDATE(),
    create_user_id    NVARCHAR(64),
    CONSTRAINT PK_t_out_side_bpm_approve_template PRIMARY KEY (id)
);



-- ----------------------------
-- Table structure for t_out_side_bpm_conditions_template
-- ----------------------------
CREATE TABLE t_out_side_bpm_conditions_template
(
    id                BIGINT IDENTITY(1,1) NOT NULL,
    business_party_id BIGINT,
    template_mark     NVARCHAR(50),
    template_name     NVARCHAR(50),
    application_id    INT,
    remark            NVARCHAR(255),
    is_del            TINYINT  DEFAULT 0,
    create_user       NVARCHAR(50),
    create_time       DATETIME DEFAULT GETDATE(),
    update_user       NVARCHAR(50),
    update_time       DATETIME DEFAULT GETDATE(),
    create_user_id    NVARCHAR(64),
    CONSTRAINT PK_t_out_side_bpm_conditions_template PRIMARY KEY (id)
);



-- ----------------------------
-- Table structure for t_out_side_bpmn_node_conditions_conf
-- ----------------------------
CREATE TABLE t_out_side_bpmn_node_conditions_conf
(
    id           BIGINT IDENTITY(1,1) NOT NULL,
    bpmn_node_id BIGINT,
    out_side_id  NVARCHAR(50),
    remark       NVARCHAR(255),
    is_del       INT,
    create_user  NVARCHAR(50),
    create_time  DATETIME DEFAULT GETDATE(),
    update_user  NVARCHAR(50),
    update_time  DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_t_out_side_bpmn_node_conditions_conf PRIMARY KEY (id)
);



-- ----------------------------
-- Table structure for t_out_side_bpm_call_back_record
-- ----------------------------
CREATE TABLE t_out_side_bpm_call_back_record
(
    id                    INT IDENTITY(1,1) NOT NULL,
    process_number        NVARCHAR(50),
    status                TINYINT,
    retry_times           TINYINT,
    button_operation_type TINYINT,
    call_back_type_name   NVARCHAR(255),
    business_id           BIGINT,
    form_code             NVARCHAR(50),
    is_del                TINYINT  DEFAULT 0,
    create_user           NVARCHAR(50),
    create_time           DATETIME DEFAULT GETDATE(),
    update_user           NVARCHAR(50),
    update_time           DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_t_out_side_bpm_call_back_record PRIMARY KEY (id)
);



-- ----------------------------
-- Table structure for t_quick_entry
-- ----------------------------
CREATE TABLE t_quick_entry
(
    id                INT IDENTITY(1,1) NOT NULL,
    title             NVARCHAR(100),
    effective_source  NVARCHAR(255),
    is_del            TINYINT  DEFAULT 0,
    tenant_id         NVARCHAR(64) DEFAULT '',
    route             NVARCHAR(500),
    sort              TINYINT  DEFAULT 0,
    create_time       DATETIME DEFAULT GETDATE(),
    status            TINYINT  DEFAULT 0,
    variable_url_flag TINYINT  DEFAULT 0,
    CONSTRAINT PK_t_quick_entry PRIMARY KEY (id)
);



-- ----------------------------
-- Table structure for t_quick_entry_type
-- ----------------------------
CREATE TABLE t_quick_entry_type
(
    id             BIGINT IDENTITY(1,1) NOT NULL,
    quick_entry_id BIGINT,
    type           INT,
    is_del         TINYINT  DEFAULT 0,
    tenant_id      NVARCHAR(64) DEFAULT '',
    create_time    DATETIME DEFAULT GETDATE(),
    type_name      NVARCHAR(255),
    CONSTRAINT PK_t_quick_entry_type PRIMARY KEY (id)
);



-- ----------------------------
-- Table structure for t_sys_version
-- ----------------------------
CREATE TABLE t_sys_version
(
    id             BIGINT IDENTITY(1,1) NOT NULL,
    create_time    DATETIME DEFAULT GETDATE(),
    update_time    DATETIME DEFAULT GETDATE(),
    is_del         TINYINT  DEFAULT 0,
    tenant_id      NVARCHAR(64) DEFAULT '',
    version        NVARCHAR(100),
    description    NVARCHAR(255), [
    index]
    INT,
    is_force       TINYINT,
    android_url    NVARCHAR(500),
    ios_url        NVARCHAR(500),
    create_user    NVARCHAR(50),
    update_user    NVARCHAR(50),
    is_hide        TINYINT,
    download_code  NVARCHAR(255),
    effective_time DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_t_sys_version PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for t_bpmn_node_role_conf
-- ----------------------------
CREATE TABLE t_bpmn_node_role_conf
(
    id           BIGINT IDENTITY(1,1) NOT NULL,
    bpmn_node_id BIGINT,
    role_id      NVARCHAR(64),
    role_name    NVARCHAR(64),
    sign_type    INT,
    remark       NVARCHAR(255),
    is_del       TINYINT  DEFAULT 0,
    tenant_id    NVARCHAR(64) DEFAULT '',
    create_user  NVARCHAR(50),
    create_time  DATETIME DEFAULT GETDATE(),
    update_user  NVARCHAR(50),
    update_time  DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpmn_node_role_conf PRIMARY KEY (id)
);



-- ----------------------------
-- Table structure for t_bpmn_node_role_outside_emp_conf
-- ----------------------------
CREATE TABLE t_bpmn_node_role_outside_emp_conf
(
    id          INT IDENTITY(1,1) NOT NULL,
    node_id     BIGINT,
    empl_id     NVARCHAR(64),
    empl_name   NVARCHAR(50),
    create_user NVARCHAR(50),
    create_time DATETIME DEFAULT GETDATE(),
    update_user NVARCHAR(255),
    update_time DATETIME DEFAULT GETDATE(),
    is_del      TINYINT  DEFAULT 0,
    tenant_id   NVARCHAR(64) DEFAULT '',
    CONSTRAINT PK_t_bpmn_node_role_outside_emp_conf PRIMARY KEY (id)
);



-- ----------------------------
-- Table structure for t_bpmn_node_loop_conf
-- ----------------------------
CREATE TABLE t_bpmn_node_loop_conf
(
    id                        BIGINT IDENTITY(1,1) NOT NULL,
    bpmn_node_id              BIGINT,
    loop_end_type             INT,
    loop_number_plies         INT,
    loop_end_person           NVARCHAR(50),
    noparticipating_staff_ids NVARCHAR(255),
    loop_end_grade            INT,
    remark                    NVARCHAR(255),
    is_del                    TINYINT  DEFAULT 0,
    tenant_id                 NVARCHAR(64) DEFAULT '',
    create_user               NVARCHAR(50),
    create_time               DATETIME DEFAULT GETDATE(),
    update_user               NVARCHAR(50),
    update_time               DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpmn_node_loop_conf PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for t_bpmn_node_assign_level_conf
-- ----------------------------
CREATE TABLE t_bpmn_node_assign_level_conf
(
    id                 BIGINT IDENTITY(1,1) NOT NULL,
    bpmn_node_id       BIGINT,
    assign_level_type  TINYINT,
    assign_level_grade TINYINT,
    remark             NVARCHAR(255),
    is_del             TINYINT  DEFAULT 0,
    tenant_id          NVARCHAR(64) DEFAULT '',
    create_user        NVARCHAR(255),
    create_time        DATETIME DEFAULT GETDATE(),
    update_user        NVARCHAR(255),
    update_time        DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpmn_node_assign_level_conf PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for t_bpmn_node_hrbp_conf
-- ----------------------------
CREATE TABLE t_bpmn_node_hrbp_conf
(
    id             BIGINT IDENTITY(1,1) NOT NULL,
    bpmn_node_id   BIGINT,
    hrbp_conf_type INT,
    remark         NVARCHAR(255),
    is_del         INT,
    tenant_id      NVARCHAR(64) DEFAULT '',
    create_user    NVARCHAR(255),
    create_time    DATETIME,
    update_user    NVARCHAR(255),
    update_time    DATETIME,
    CONSTRAINT PK_t_bpmn_node_hrbp_conf PRIMARY KEY (id)
);



-- ----------------------------
-- Table structure for t_department
-- ----------------------------
CREATE TABLE t_department
(
    id          BIGINT IDENTITY(1,1) NOT NULL,
    name        NVARCHAR(255),
    short_name  NVARCHAR(255),
    parent_id   INT,
    path        NVARCHAR(255),
    level       INT,
    leader_id   BIGINT,
    sort        INT,
    is_del      TINYINT,
    is_hide     TINYINT,
    create_user NVARCHAR(255),
    update_user NVARCHAR(255),
    create_time DATETIME,
    update_time DATETIME,
    CONSTRAINT PK_t_department PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for t_user
-- ----------------------------
CREATE TABLE t_user
(
    id             BIGINT IDENTITY(1,1) NOT NULL,
    user_name      NVARCHAR(255),
    mobile         NVARCHAR(50),
    email          NVARCHAR(50),
    leader_id      BIGINT,
    hrbp_id        BIGINT,
    mobile_is_show TINYINT DEFAULT 0,
    department_id  BIGINT,
    path           NVARCHAR(1000),
    is_del         TINYINT DEFAULT 0,
    head_img       NVARCHAR(3000),
    CONSTRAINT PK_t_user PRIMARY KEY (id)
);



-- ----------------------------
-- Table structure for t_role
-- ----------------------------
CREATE TABLE t_role
(
    id        BIGINT IDENTITY(1,1) NOT NULL,
    role_name NVARCHAR(255),
    CONSTRAINT PK_t_role PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for t_biz_leavetime
-- ----------------------------
CREATE TABLE t_biz_leavetime
(
    id              BIGINT IDENTITY(1,1) NOT NULL,
    leave_user_id   BIGINT,
    leave_user_name NVARCHAR(255),
    leave_type      INT,
    begin_time      DATETIME,
    end_time        DATETIME,
    leavehour       FLOAT,
    remark          NVARCHAR(255),
    create_user     NVARCHAR(255),
    create_time     DATETIME DEFAULT GETDATE(),
    update_user     NVARCHAR(255),
    update_time     DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_t_biz_leavetime PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for t_biz_purchase
-- ----------------------------
CREATE TABLE t_biz_purchase
(
    id                           BIGINT IDENTITY(1,1) NOT NULL,
    purchase_user_id             BIGINT,
    purchase_user_name           NVARCHAR(255),
    purchase_type                INT      DEFAULT 1,
    purchase_time                DATETIME DEFAULT GETDATE(),
    plan_procurement_total_money FLOAT    DEFAULT 0,
    remark                       NVARCHAR(255),
    create_user                  NVARCHAR(255),
    create_time                  DATETIME DEFAULT GETDATE(),
    update_user                  NVARCHAR(255),
    update_time                  DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_t_biz_purchase PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for t_biz_ucar_refuel
-- ----------------------------
CREATE TABLE t_biz_ucar_refuel
(
    id                   BIGINT IDENTITY(1,1) NOT NULL,
    license_plate_number NVARCHAR(32),
    refuel_time          DATETIME,
    remark               NVARCHAR(255),
    create_user          NVARCHAR(50),
    create_time          DATETIME DEFAULT GETDATE(),
    update_user          NVARCHAR(50),
    update_time          DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_t_biz_ucar_refuel PRIMARY KEY (id)
);



-- ----------------------------
-- Table structure for t_biz_refund
-- ----------------------------
CREATE TABLE t_biz_refund
(
    id               BIGINT IDENTITY(1,1) NOT NULL,
    refund_user_id   BIGINT,
    refund_user_name NVARCHAR(255),
    refund_type      INT            DEFAULT 1,
    refund_date      DATETIME,
    refund_money     DECIMAL(18, 2) DEFAULT 0,
    remark           NVARCHAR(255),
    create_user      NVARCHAR(255),
    create_time      DATETIME       DEFAULT GETDATE(),
    update_user      NVARCHAR(255),
    update_time      DATETIME       DEFAULT GETDATE(),
    CONSTRAINT PK_t_biz_refund PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for t_bpmn_conf_lf_formdata
-- ----------------------------
CREATE TABLE t_bpmn_conf_lf_formdata
(
    id           BIGINT IDENTITY(1,1) NOT NULL,
    bpmn_conf_id BIGINT,
    formdata     NVARCHAR(MAX),
    is_del       TINYINT  DEFAULT 0,
    tenant_id    NVARCHAR(64) DEFAULT '',
    create_user  NVARCHAR(255),
    create_time  DATETIME DEFAULT GETDATE(),
    update_user  NVARCHAR(255),
    update_time  DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpmn_conf_lf_formdata PRIMARY KEY (id)
);



-- ----------------------------
-- Table structure for t_bpmn_conf_lf_formdata_field
-- ----------------------------
CREATE TABLE t_bpmn_conf_lf_formdata_field
(
    id           BIGINT IDENTITY(1,1) NOT NULL,
    bpmn_conf_id BIGINT,
    formdata_id  BIGINT,
    field_id     NVARCHAR(255),
    field_name   NVARCHAR(255),
    field_type   TINYINT,
    is_condition TINYINT  DEFAULT 0,
    is_del       TINYINT  DEFAULT 0,
    tenant_id    NVARCHAR(64) DEFAULT '',
    create_user  NVARCHAR(255),
    create_time  DATETIME DEFAULT GETDATE(),
    update_user  NVARCHAR(255),
    update_time  DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpmn_conf_lf_formdata_field PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for t_bpmn_node_lf_formdata_field_control
-- ----------------------------
CREATE TABLE t_bpmn_node_lf_formdata_field_control
(
    id          BIGINT IDENTITY(1,1) NOT NULL,
    node_id     BIGINT,
    formdata_id BIGINT,
    field_id    NVARCHAR(100),
    field_name  NVARCHAR(255),
    field_perm  NVARCHAR(10),
    is_del      TINYINT  DEFAULT 0,
    tenant_id   NVARCHAR(64) DEFAULT '',
    create_user NVARCHAR(255),
    create_time DATETIME DEFAULT GETDATE(),
    update_user NVARCHAR(255),
    update_time DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpmn_node_lf_formdata_field_control PRIMARY KEY (id)
);



-- ----------------------------
-- Table structure for t_lf_main
-- ----------------------------
CREATE TABLE t_lf_main
(
    id          BIGINT IDENTITY(1,1) NOT NULL,
    conf_id     BIGINT,
    form_code   NVARCHAR(255),
    is_del      TINYINT  DEFAULT 0,
    tenant_id   NVARCHAR(64) DEFAULT '',
    create_user NVARCHAR(255),
    create_time DATETIME DEFAULT GETDATE(),
    update_user NVARCHAR(255),
    update_time DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_t_lf_main PRIMARY KEY (id)
);



-- ----------------------------
-- Table structure for t_lf_main_field
-- ----------------------------
CREATE TABLE t_lf_main_field
(
    id                 BIGINT IDENTITY(1,1) NOT NULL,
    main_id            BIGINT,
    form_code          NVARCHAR(255),
    field_id           NVARCHAR(255),
    field_name         NVARCHAR(255),
    parent_field_id    NVARCHAR(255),
    parent_field_name  NVARCHAR(255),
    field_value        NVARCHAR(2000),
    field_value_number DECIMAL(14, 2),
    field_value_dt     DATETIME,
    field_value_text   NVARCHAR(MAX),
    sort               INT      DEFAULT 0,
    is_del             TINYINT  DEFAULT 0,
    tenant_id          NVARCHAR(64) DEFAULT '',
    create_user        NVARCHAR(255),
    create_time        DATETIME DEFAULT GETDATE(),
    update_user        NVARCHAR(255),
    update_time        DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_t_lf_main_field PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for t_dict_main
-- ----------------------------
CREATE TABLE t_dict_main
(
    id          BIGINT IDENTITY(1,1) NOT NULL,
    dict_name   NVARCHAR(100) DEFAULT '',
    dict_type   NVARCHAR(100) DEFAULT '',
    is_del      TINYINT  DEFAULT 0,
    tenant_id   NVARCHAR(64) DEFAULT '',
    create_user NVARCHAR(255),
    create_time DATETIME DEFAULT GETDATE(),
    update_user NVARCHAR(255),
    update_time DATETIME DEFAULT GETDATE(),
    remark      NVARCHAR(500),
    CONSTRAINT PK_t_dict_main PRIMARY KEY (id),
    CONSTRAINT UK_t_dict_main_dict_type UNIQUE (dict_type)
);


-- ----------------------------
-- Table structure for t_dict_data
-- ----------------------------
CREATE TABLE t_dict_data
(
    id          BIGINT IDENTITY(1,1) NOT NULL,
    dict_sort   INT      DEFAULT 0,
    dict_label  NVARCHAR(100) DEFAULT '',
    dict_value  NVARCHAR(100) DEFAULT '',
    dict_type   NVARCHAR(100) DEFAULT '',
    css_class   NVARCHAR(100),
    list_class  NVARCHAR(100),
    is_default  NCHAR(1) DEFAULT 'N',
    is_del      TINYINT  DEFAULT 0,
    tenant_id   NVARCHAR(64) DEFAULT '',
    create_user NVARCHAR(255),
    create_time DATETIME DEFAULT GETDATE(),
    update_user NVARCHAR(255),
    update_time DATETIME DEFAULT GETDATE(),
    remark      NVARCHAR(500),
    CONSTRAINT PK_t_dict_data PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX idx_processInstance_Id
ON bpm_process_node_submit (processInstance_Id);


CREATE TABLE t_user_role
(
    id      INT IDENTITY(1,1) NOT NULL,
    user_id INT,
    role_id INT,
    CONSTRAINT PK_t_user_role PRIMARY KEY (id)
);



-- ----------------------------
-- Table structure for t_bpmn_node_labels
-- ----------------------------
CREATE TABLE t_bpmn_node_labels
(
    id          BIGINT IDENTITY(1,1) NOT NULL,
    nodeid      BIGINT,
    label_name  NVARCHAR(50),
    label_value NVARCHAR(64),
    remark      NVARCHAR(255) DEFAULT '',
    is_del      TINYINT  DEFAULT 0,
    tenant_id   NVARCHAR(64) DEFAULT '',
    create_user NVARCHAR(32) DEFAULT '',
    create_time DATETIME DEFAULT GETDATE(),
    update_user NVARCHAR(32) DEFAULT '',
    update_time DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_t_bpmn_node_labels PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX indx_node_id
ON t_bpmn_node_labels (nodeid);


-- ----------------------------
-- Table structure for t_bpm_dynamic_condition_choosen
-- ----------------------------
CREATE TABLE t_bpm_dynamic_condition_choosen
(
    id             BIGINT IDENTITY(1,1) NOT NULL,
    process_number NVARCHAR(255),
    node_id        NVARCHAR(100),
    node_from      NVARCHAR(100),
    is_del         TINYINT DEFAULT 0,
    tenant_id      NVARCHAR(64) DEFAULT '',
    CONSTRAINT PK_t_bpm_dynamic_condition_choosen PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX indx_process_number
ON t_bpm_dynamic_condition_choosen (process_number);




-- ----------------------------
-- Table structure for t_bpmn_node_customize_conf
-- ----------------------------
CREATE TABLE t_bpmn_node_customize_conf
(
    id           BIGINT IDENTITY(1,1) NOT NULL,
    bpmn_node_id BIGINT,
    sign_type    INT,
    remark       NVARCHAR(255),
    is_del       INT,
    tenant_id    NVARCHAR(255) DEFAULT '',
    create_user  NVARCHAR(255),
    create_time  DATETIME,
    update_user  NVARCHAR(255),
    update_time  DATETIME,
    CONSTRAINT PK_t_bpmn_node_customize_conf PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for bpm_af_deployment
-- ----------------------------
CREATE TABLE bpm_af_deployment
(
    id          NVARCHAR(64) NOT NULL,
    rev         INT,
    name        NVARCHAR(255),
    content     NVARCHAR(MAX),
    remark      NVARCHAR(255) DEFAULT '',
    is_del      TINYINT  DEFAULT 0,
    tenant_id   NVARCHAR(64) DEFAULT '',
    create_user NVARCHAR(32) DEFAULT '',
    create_time DATETIME DEFAULT GETDATE(),
    update_user NVARCHAR(32) DEFAULT '',
    update_time DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_bpm_af_deployment PRIMARY KEY (id)
);


-- ----------------------------
-- Table structure for bpm_af_taskinst
-- ----------------------------
CREATE TABLE bpm_af_taskinst
(
    id                     NVARCHAR(64) NOT NULL,
    proc_def_id            NVARCHAR(64),
    task_def_key           NVARCHAR(255),
    proc_inst_id           NVARCHAR(64),
    execution_id           NVARCHAR(64),
    name                   NVARCHAR(255),
    parent_task_id         NVARCHAR(64),
    owner                  NVARCHAR(255),
    assignee               NVARCHAR(255),
    assignee_name          NVARCHAR(255),
    original_assignee      NVARCHAR(255),
    original_assignee_name NVARCHAR(255),
    transfer_reason        NVARCHAR(1000),
    verify_status          INT,
    verify_desc            NVARCHAR(2000),
    start_time             DATETIME2(3),
    claim_time             DATETIME2(3),
    end_time               DATETIME2(3),
    duration               BIGINT,
    delete_reason          NVARCHAR(4000),
    priority               INT,
    due_date               DATETIME2(3),
    form_key               NVARCHAR(255),
    category               NVARCHAR(255),
    tenant_id              NVARCHAR(255) DEFAULT '',
    description            NVARCHAR(4000),
    update_user            NVARCHAR(64),
    CONSTRAINT PK_bpm_af_taskinst PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX AF_HI_TASK_INST_PROCINST 

ON bpm_af_taskinst (proc_inst_id);


CREATE
NONCLUSTERED INDEX idx_assignee_name 

ON bpm_af_taskinst (assignee_name);


CREATE
NONCLUSTERED INDEX idx_task_def_key 

ON bpm_af_taskinst (task_def_key);
       
       
-- ----------------------------
-- Table structure for bpm_af_task
-- ----------------------------
CREATE TABLE bpm_af_task
(
    id               NVARCHAR(64) NOT NULL,
    rev              INT,
    execution_id     NVARCHAR(64),
    proc_inst_id     NVARCHAR(64),
    proc_def_id      NVARCHAR(64),
    name             NVARCHAR(255),
    parent_task_id   NVARCHAR(64),
    task_def_key     NVARCHAR(255),
    owner            NVARCHAR(255),
    assignee         NVARCHAR(255),
    assignee_name    NVARCHAR(255),
    node_id          NVARCHAR(64),
    node_type        INT,
    delegation       NVARCHAR(64),
    priority         INT,
    create_time      DATETIME2(3),
    due_date         DATETIME2(3),
    category         NVARCHAR(255),
    suspension_state INT,
    tenant_id        NVARCHAR(255) DEFAULT '',
    form_key         NVARCHAR(255),
    description      NVARCHAR(4000),
    CONSTRAINT PK_bpm_af_task PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX AF_IDX_TASK_CREATE 
ON bpm_af_task (create_time);

CREATE
NONCLUSTERED INDEX AF_IDX_PROCINSTID 
ON bpm_af_task (proc_inst_id);

CREATE
NONCLUSTERED INDEX AF_IDX_TASK_DEF_KEY 
ON bpm_af_task (task_def_key);

CREATE
NONCLUSTERED INDEX AF_IDX_TASK_ASSIGNEE 
ON bpm_af_task (assignee);
       
       
-- ----------------------------
-- Table structure for bpm_af_execution
-- ----------------------------
CREATE TABLE bpm_af_execution
(
    id                NVARCHAR(64) NOT NULL,
    rev_              INT,
    proc_inst_id      NVARCHAR(64),
    business_key      NVARCHAR(255),
    parent_id         NVARCHAR(64),
    proc_def_id       NVARCHAR(64),
    super_exec        NVARCHAR(64),
    root_proc_inst_id NVARCHAR(64),
    act_id            NVARCHAR(255),
    is_active         TINYINT,
    is_concurrent     TINYINT,
    tenant_id         NVARCHAR(255) DEFAULT '',
    name              NVARCHAR(255),
    start_time        DATETIME,
    start_user_id     NVARCHAR(255),
    is_count_enabled  TINYINT,
    evt_subscr_count  INT,
    task_count        INT,
    var_count         INT,
    sign_type         INT,
    CONSTRAINT PK_bpm_af_execution PRIMARY KEY (id)
);

CREATE
NONCLUSTERED INDEX AF_IDX_EXEC_PROCINSTID 
ON bpm_af_execution (proc_inst_id);

CREATE
NONCLUSTERED INDEX AF_IDX_EXEC_BUSKEY 
ON bpm_af_execution (business_key);


-- ----------------------------
-- Records of t_user
-- 关于用户表demo数据的使用说明
-- t_user,t_role,t_department表都是测试数据,实际使用中,一般用户系统里面都会有基本的用户表,角色表,部门表,审批的时候用户可以根据实际情况去使用或者关联使用自己已有系统的表,选择出来的数据结构符合BaseIdTranStruVo结构即可,即有id和name两个字段
-- 初次使用时,用户可以先初始化demo表,看一下流程是否满足自己的业务需求,然后逐步改sql,满足自己的业务需求.Antflow demo里审批人规则特别多,实际上用户可能只需要一个或者多个规则(一般指定人员,直属领导,直接角色就满足了),根据需求实现部分即可,像hrbp有的公司根本没有这个概念,自然也没必要实现
-- 用户实现审批人规则时,查看PersonnelEnum枚举,参照指定人员来实现其它的,实现无非就是改写sql而已,其实很简单,很多用户绕不过来,以为自己不熟悉antflow就不敢改,只要返回的数据结构符合BaseIdTranStruVo实体即可
-- ----------------------------

INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('张三', NULL, 'zypqqgc@qq.com', 13, 17, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('李四', NULL, 'zypqqgc@qq.com', 13, 17, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('王五', NULL, 'zypqqgc@qq.com', 13, 17, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('菜六', NULL, 'zypqqgc@qq.com', 13, 17, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('牛七', NULL, 'zypqqgc@qq.com', 13, 17, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('马八', NULL, 'zypqqgc@qq.com', 13, 17, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('李九', NULL, 'zypqqgc@qq.com', 13, 17, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('周十', NULL, 'zypqqgc@qq.com', 13, 17, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('肖十一', NULL, 'zypqqgc@qq.com', 13, 17, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('令狐冲', NULL, 'zypqqgc@qq.com', 13, 17, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('风清扬', NULL, 'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('刘正风', NULL, 'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('岳不群', NULL, 'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('宁中则', NULL, 'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('桃谷六仙', NULL, 'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('不介和尚', NULL, 'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('丁一师太', NULL, 'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('依林师妹', NULL, 'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('邱灵珊', NULL, 'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('任盈盈', NULL, 'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('斯克', NULL, 'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('川普', NULL, 'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO t_user(user_name, mobile, email, leader_id, hrbp_id, mobile_is_show, path, is_del, head_img, department_id)
VALUES ('小马', NULL, 'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);


-- ----------------------------

-- Records of t_user_role

-- ----------------------------

INSERT INTO t_user_role(user_id, role_id)
VALUES (1, 1);
INSERT INTO t_user_role(user_id, role_id)
VALUES (1, 1);
INSERT INTO t_user_role(user_id, role_id)
VALUES (1, 3);
INSERT INTO t_user_role(user_id, role_id)
VALUES (2, 2);
INSERT INTO t_user_role(user_id, role_id)
VALUES (2, 3);
INSERT INTO t_user_role(user_id, role_id)
VALUES (2, 3);
INSERT INTO t_user_role(user_id, role_id)
VALUES (3, 3);
INSERT INTO t_user_role(user_id, role_id)
VALUES (4, 3);
INSERT INTO t_user_role(user_id, role_id)
VALUES (5, 3);
INSERT INTO t_user_role(user_id, role_id)
VALUES (6, 3);
INSERT INTO t_user_role(user_id, role_id)
VALUES (7, 3);
INSERT INTO t_user_role(user_id, role_id)
VALUES (11, 3);
INSERT INTO t_user_role(user_id, role_id)
VALUES (10, 6);
INSERT INTO t_user_role(user_id, role_id)
VALUES (8, 7);
INSERT INTO t_user_role(user_id, role_id)
VALUES (19, 8);
INSERT INTO t_user_role(user_id, role_id)
VALUES (12, 4);
INSERT INTO t_user_role(user_id, role_id)
VALUES (13, 5);
INSERT INTO t_user_role(user_id, role_id)
VALUES (16, 4);


-- ----------------------------

-- Records of t_department

-- ----------------------------

INSERT INTO t_department(name, short_name, parent_id, path, level, leader_id)
VALUES ('一级部门', NULL, NULL, '/1', 1, 1);

INSERT INTO t_department(name, short_name, parent_id, path, level, leader_id)
VALUES ('二级部门', NULL, 3, '/1/2', 2, 2);

INSERT INTO t_department(name, short_name, parent_id, path, level, leader_id)
VALUES ('三级部门', NULL, 4, '/1/2/3', 3, 3);

INSERT INTO t_department(name, short_name, parent_id, path, level, leader_id)
VALUES ('四级部门', NULL, 5, '/1/2/3/4', 4, 4);

INSERT INTO t_department(name, short_name, parent_id, path, level, leader_id)
VALUES ('五级部门', NULL, 6, '/1/2/3/4/5', 5, 5);

INSERT INTO t_department(name, short_name, parent_id, path, level, leader_id)
VALUES ('六级部门', NULL, 7, '/1/2/3/4/5/6', 6, 6);

INSERT INTO t_department(name, short_name, parent_id, path, level, leader_id)
VALUES ('七级部门', NULL, 8, '/1/2/3/4/5/6/7', 7, 7);

INSERT INTO t_department(name, short_name, parent_id, path, level, leader_id)
VALUES ('市场部', NULL, 9, '/1/2/3/4/5/6/7/8', 8, 8);

INSERT INTO t_department(name, short_name, parent_id, path, level, leader_id)
VALUES ('销售部', NULL, 9, '/1/2/3/4/5/6/7/8/9', 9, 9);

