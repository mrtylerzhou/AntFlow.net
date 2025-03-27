CREATE TABLE if not exists `t_bpmn_conf`
(
    `id`                  int(11)             NOT NULL AUTO_INCREMENT COMMENT 'Auto Incr ID',
    `bpmn_code`           varchar(60)         NOT NULL DEFAULT '' COMMENT 'Process code',
    `bpmn_name`           varchar(60)         NOT NULL DEFAULT '' COMMENT 'Process Name',
    `bpmn_type`           int(11)                      DEFAULT NULL COMMENT 'Process Type',
    `form_code`           varchar(100)        NOT NULL DEFAULT '' COMMENT 'Process Business Code',
    `app_id`              int(11)                      DEFAULT NULL COMMENT 'associated app id',
    `deduplication_type`  int(11)             NOT NULL DEFAULT '1' COMMENT 'deduplication way 1.no deduplication,2 forward deduplication,3.backward deduplication',
    `effective_status`    int(11)             NOT NULL DEFAULT '0' COMMENT 'is effect 0:no 1:yes',
    `is_all`              int(11)             NOT NULL DEFAULT '0' COMMENT 'is to all,0 no 1yes',
    `is_out_side_process` int(11)                      DEFAULT '0' COMMENT 'is it a third party process',
    `is_lowcode_flow` tinyint default 0 null comment '是否是低代码审批流0,否,1是',
    `business_party_id`   int(11)                      DEFAULT NULL COMMENT 'its belong to business party',
    `extra_flags`         int                                           null,
    `remark`              varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`              tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:in use,1:delete',
    `create_user`         varchar(32)                  DEFAULT '' COMMENT 'as its name says',
    `create_time`         timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user`         varchar(32)                  DEFAULT '' COMMENT '更新人',
    `update_time`         timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE,
    UNIQUE KEY `bpmn_code` (`bpmn_code`) USING BTREE,
    KEY `index_business_party_id` (`business_party_id`) USING BTREE,
    KEY `index_form_code` (`form_code`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='process main configuration table';

CREATE TABLE if not exists `t_bpmn_node`
(
    `id`                bigint(20)          NOT NULL AUTO_INCREMENT COMMENT 'id',
    `conf_id`           bigint(20)          NOT NULL COMMENT 'the main conf id',
    `node_id`           varchar(60)         NOT NULL DEFAULT '' COMMENT 'node id',
    `node_type`         int(11)             NOT NULL COMMENT 'its node type,see NodeTypeEnum for detail',
    `node_property`     int(11)             NOT NULL COMMENT 'node property,rules for finding out approvers,see NodePropertyEnum for detail',
    `node_from`         varchar(60)         NOT NULL DEFAULT '' COMMENT 'its prev node',
    `node_froms`        varchar(255)                 DEFAULT NULL COMMENT 'all its prev nodes',
    `batch_status`      int(11)             NOT NULL DEFAULT '0' COMMENT 'can the process approved in batch,0:no,1:Yes',
    `approval_standard` int(11)             NOT NULL DEFAULT '2' COMMENT 'approve standard,current not used',
    `node_name`         varchar(255)                 DEFAULT NULL COMMENT 'node name',
    `node_display_name` varchar(255)                  DEFAULT '' COMMENT 'node display name shown in web or app',
    `annotation`        varchar(255)                 DEFAULT NULL COMMENT 'annotation on this conf',
    `is_deduplication`  int(11)             NOT NULL DEFAULT '0' COMMENT 'whether this node should be deduplicated,0:No,1:Yes',
    `deduplicationExclude` tinyint             default 0                 null comment '0 for no,default value,and 1 for yes',
    `is_dynamicCondition` tinyint default 0 not null comment '是否是动态条件节点,0,否,1是',
    `is_sign_up`        int(11)             NOT NULL DEFAULT '0' COMMENT 'whether this node can be sign up,0:No,1:Yes',
    `remark`            varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`            tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:No,1:yes',
    `create_user`       varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `create_time`       timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user`       varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `update_time`       timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE,
    KEY `index_conf_id` (`conf_id`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='the conf,s node table';

CREATE TABLE if not exists `t_bpmn_node_business_table_conf`
(
    `id`                       int(11)             NOT NULL AUTO_INCREMENT COMMENT 'id',
    `bpmn_node_id`             bigint(20)          NOT NULL COMMENT 'node id',
    `configuration_table_type` int(11)                      DEFAULT NULL COMMENT 'conf table type',
    `table_field_type`         int(11)                      DEFAULT NULL COMMENT 'table field type',
    `sign_type`                int(11)             NOT NULL COMMENT 'ign type 1:all sign,2: or sign',
    `remark`                   varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`                   tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:no,1:yes',
    `create_user`              varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `create_time`              timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user`              varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `update_time`              timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE,
    KEY `index_node_id` (`bpmn_node_id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='node conf,s business conf table';

CREATE TABLE if not exists `t_bpmn_conf_notice_template`
(
    `id`          int(8)              NOT NULL AUTO_INCREMENT,
    `bpmn_code`   varchar(60)         NOT NULL DEFAULT '' COMMENT 'bpmn code',
    `is_del`      tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:no,1:yes',
    `create_user` varchar(50)                  DEFAULT '' COMMENT '创建人',
    `create_time` timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user` varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `update_time` timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE,
    KEY `index_bpmn_code` (`bpmn_code`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='process,s notice template';

CREATE TABLE if not exists `t_bpmn_view_page_button`
(
    `id`          int(11)             NOT NULL AUTO_INCREMENT COMMENT 'id',
    `conf_id`     int(11)             NOT NULL COMMENT 'conf id',
    `view_type`   int(11)             NOT NULL COMMENT 'view type 1:start use view,2:approvers view',
    `button_type` int(11)             NOT NULL COMMENT 'button type,see ButtonTypeEnum for details',
    `button_name` varchar(60)                  DEFAULT '' COMMENT 'button,s name',
    `remark`      varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`      tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:no,1:yes',
    `create_user` varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `create_time` timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user` varchar(50)                  DEFAULT '' COMMENT '更新人（邮箱前缀）',
    `update_time` timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='审批流查看页按钮配置表';

CREATE TABLE if not exists `t_bpmn_template`
(
    `id`          int(11)    NOT NULL AUTO_INCREMENT,
    `conf_id`     bigint(20)          DEFAULT NULL COMMENT 'conf,s id',
    `node_id`     bigint(20)          DEFAULT NULL COMMENT 'node,s id',
    `event`       int(11)             DEFAULT NULL COMMENT 'event type',
    `informs`     varchar(255)        DEFAULT NULL COMMENT 'who to inform',
    `emps`        varchar(255)        DEFAULT NULL COMMENT 'specified person to inform',
    `roles`       varchar(255)        DEFAULT NULL COMMENT 'specified roles to inform',
    `funcs`       varchar(255)        DEFAULT NULL COMMENT 'specified Functionality to inform',
    `template_id` bigint(20)          DEFAULT NULL COMMENT 'template id id',
    `is_del`      tinyint(4) NOT NULL DEFAULT '0' COMMENT '0:No,1:Yes',
    `create_time` timestamp  NULL     DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `create_user` varchar(50)        DEFAULT '' COMMENT 'create user',
    `update_time` timestamp  NULL     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user` varchar(50)        DEFAULT '' COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='notice template';

CREATE TABLE if not exists `t_information_template`
(
    `id`             bigint(20)   NOT NULL AUTO_INCREMENT,
    `name`           varchar(30)  NOT NULL DEFAULT '' COMMENT 'name',
    `num`            varchar(10)  NOT NULL DEFAULT '' COMMENT 'num',
    `system_title`   varchar(100) NOT NULL DEFAULT '' COMMENT 'title',
    `system_content` varchar(500) NOT NULL DEFAULT '' COMMENT 'content',
    `mail_title`     varchar(100) NOT NULL DEFAULT '' COMMENT 'mail title',
    `mail_content`   varchar(500) NOT NULL DEFAULT '' COMMENT 'mail content',
    `note_content`   varchar(200) NOT NULL DEFAULT '' COMMENT 'sms content',
    `jump_url`       int(11)               DEFAULT NULL COMMENT 'url to jump to',
    `remark`         varchar(200) NOT NULL DEFAULT '' COMMENT 'remark',
    `status`         tinyint(4)   NOT NULL DEFAULT '0' COMMENT 'status 0:in use,1:disabled',
    `is_del`         tinyint(4)   NOT NULL DEFAULT '0' COMMENT '0:no,1:yes',
    `create_time`    timestamp    NOT NULL     DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `create_user`    varchar(50)          DEFAULT '' COMMENT 'as its name says',
    `update_time`    timestamp    NOT NULL     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user`    varchar(50)          DEFAULT '' COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='消息模板';

CREATE TABLE if not exists `bpm_business`
(
    `id`               bigint(20) NOT NULL AUTO_INCREMENT,
    `business_id`      varchar(64)   DEFAULT NULL COMMENT 'business id',
    `create_time`      timestamp    not null default CURRENT_TIMESTAMP COMMENT 'as its name says',
    `process_code`     varchar(50)  DEFAULT NULL COMMENT 'process Number',
    `create_user_name` varchar(50)  DEFAULT NULL COMMENT 'as its name says',
    `create_user`      varchar(50)   DEFAULT NULL COMMENT 'as its name says',
    `process_key`      varchar(50) DEFAULT NULL COMMENT 'as its name says',
    `is_del`           int(11)      DEFAULT '0',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='process draft';

CREATE TABLE if not exists `bpm_flowrun_entrust`
(
    `id`          int(11) NOT NULL AUTO_INCREMENT,
    `runinfoid`   varchar(64)      DEFAULT NULL COMMENT 'process instance id',
    `runtaskid`   varchar(64)      DEFAULT NULL COMMENT 'task id',
    `original`    varchar(64)          DEFAULT NULL COMMENT 'original assignee',
    `original_name`    varchar(255)          DEFAULT NULL COMMENT 'original assignee name',
    `actual`      varchar(64)          DEFAULT NULL COMMENT 'actual assignee',
    `actual_name`   varchar(100)  null comment 'actual assignee name',
    `type`        int(20)          DEFAULT NULL COMMENT 'type 1: entrust 2:view',
    `is_read`     int(11)          DEFAULT '2' COMMENT 'is read 1:yes,2:no',
    `proc_def_id` varchar(100)     DEFAULT NULL COMMENT 'proces deployment id',
    `is_view`     int(11) NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`) USING BTREE,
    KEY `BPM_IDX_ID` (`runinfoid`, `original`, `actual`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='entrust and forward view conf table';


CREATE TABLE if not exists `bpm_flowruninfo`
(
    `id`            bigint(20) NOT NULL AUTO_INCREMENT COMMENT 'ID',
    `runinfoid`     varchar(64) NOT NULL COMMENT 'process instance id',
    `create_UserId` varchar(64)   DEFAULT NULL COMMENT 'userid',
    `entitykey`     varchar(100) DEFAULT NULL COMMENT 'business key',
    `entityclass`   varchar(100) DEFAULT NULL COMMENT 'entity class',
    `entitykeytype` varchar(10)  DEFAULT NULL COMMENT 'business type',
    `createactor`   varchar(50)  DEFAULT NULL COMMENT 'create user',
    `createdepart`  varchar(100) DEFAULT NULL COMMENT 'create user,s depart',
    `createdate`    timestamp  NOT NULL     DEFAULT CURRENT_TIMESTAMP NULL COMMENT 'create time',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='process run time info';

CREATE TABLE if not exists `bpm_manual_notify`
(
    `id`          int(11)     NOT NULL AUTO_INCREMENT,
    `business_id` bigint(20)  NOT NULL COMMENT 'business id',
    `code`        varchar(10) NOT NULL COMMENT 'process type',
    `last_time`   timestamp   NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'latest remind time',
    `create_time` timestamp   NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `update_time` timestamp   NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='manual notify';

CREATE TABLE if not exists `t_bpmn_approve_remind`
(
    `id`          bigint(20) NOT NULL AUTO_INCREMENT,
    `conf_id`     bigint(20)          DEFAULT NULL COMMENT 'conf id',
    `node_id`     bigint(20)          DEFAULT NULL COMMENT 'node id',
    `template_id` bigint(20)          DEFAULT NULL COMMENT 'template id',
    `days`        varchar(255)        DEFAULT NULL COMMENT 'durations in day',
    `is_del`      tinyint(4) NOT NULL DEFAULT '0' COMMENT '0:No,1:Yes',
    `create_time` timestamp  NULL     DEFAULT CURRENT_TIMESTAMP COMMENT 'create time',
    `create_user` varchar(50)        DEFAULT '' COMMENT 'create user',
    `update_time` timestamp  NOT NULL     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'update time',
    `update_user` varchar(50)        DEFAULT '' COMMENT 'update user',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='approvement remind';

CREATE TABLE if not exists `t_bpmn_conf_notice_template_detail`
(
    `id`                     int(8)              NOT NULL AUTO_INCREMENT,
    `bpmn_code`              varchar(60)         NOT NULL DEFAULT '' COMMENT 'bpmn Code',
    `notice_template_type`   int(4)              NOT NULL DEFAULT '1' COMMENT 'notice type',
    `notice_template_detail` varchar(512)                 DEFAULT NULL COMMENT 'content',
    `is_del`                 tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:no,1:yes',
    `create_user`            varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `create_time`            timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user`            varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `update_time`            timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE,
    KEY `index_bpmn_code` (`bpmn_code`) USING BTREE,
    KEY `index_bpmn_type` (`notice_template_type`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='notice template detail';


CREATE TABLE if not exists `t_bpmn_node_conditions_conf`
(
    `id`           bigint(20)          NOT NULL AUTO_INCREMENT COMMENT 'id',
    `bpmn_node_id` bigint(20)          NOT NULL COMMENT 'conf id',
    `is_default`   int(11)             NOT NULL DEFAULT '0' COMMENT 'is default 0:no,1:yes',
    `sort`         int(11)             NOT NULL COMMENT 'condition,s priority',
    ext_json     varchar(2000)                                 null comment '前端vue3版本conditionlist参数模型',
    `remark`       varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`       tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:no,1:yes',
    `create_user`  varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `create_time`  timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user`  varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `update_time`  timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='node,s conditions conf';

CREATE TABLE if not exists `t_bpmn_node_conditions_param_conf`
(
    `id`                      bigint(20)          NOT NULL AUTO_INCREMENT COMMENT 'id',
    `bpmn_node_conditions_id` bigint(20)          NOT NULL COMMENT 'conf id',
    `condition_param_type`    int(11)             NOT NULL COMMENT 'param type,used to determine ConditionTypeEnum',
    `condition_param_name`  varchar(50)             NOT NULL COMMENT 'param field name',
    `condition_param_jsom`    text                NOT NULL COMMENT 'paramJSON',
    `remark`                  varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`                  tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:no,1:yes',
    `create_user`             varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `create_time`             timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user`             varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `update_time`             timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='condition params conf';

CREATE TABLE if not exists `t_bpmn_node_sign_up_conf`
(
    `id`                bigint(20)          NOT NULL AUTO_INCREMENT COMMENT 'id',
    `bpmn_node_id`      bigint(20)          NOT NULL COMMENT '节点配置表Id',
    `after_sign_up_way` int(11)             NOT NULL DEFAULT '1' COMMENT 'sign up flow way 1: go back to assigner,2:no',
    `sign_up_type`      int(11)             NOT NULL DEFAULT '1' COMMENT 'sign up way 1: all sign in sequencial,2:all sign,3:or sign',
    `remark`            varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`            tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:no,1:yes',
    `create_user`       varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `create_time`       timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user`       varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `update_time`       timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='node sign up conf';

CREATE TABLE if not exists `t_bpmn_node_to`
(
    `id`           bigint(20)          NOT NULL AUTO_INCREMENT COMMENT 'id',
    `bpmn_node_id` bigint(20)          NOT NULL COMMENT 'node id',
    `node_to`      varchar(60)         NOT NULL DEFAULT '' COMMENT 'node to',
    `remark`       varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`       tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:no,1:yes',
    `create_user`  varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `create_time`  timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user`  varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `update_time`  timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='审批流节点走向表';


CREATE TABLE if not exists `bpm_process_dept`
(
    `id`           bigint(20) NOT NULL AUTO_INCREMENT COMMENT 'id',
    `process_code` varchar(50)         DEFAULT NULL COMMENT 'process number',
    `process_type` int(11)             DEFAULT NULL COMMENT 'process type',
    `process_name` varchar(50)         DEFAULT NULL COMMENT 'process name',
    `dep_id`       bigint(20)          DEFAULT NULL COMMENT 'dept id',
    `remarks`      varchar(255)        DEFAULT NULL COMMENT 'remark',
    `create_time`  timestamp  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `create_user`  bigint(20)          DEFAULT NULL COMMENT 'as its name says',
    `update_user`  bigint(20)          DEFAULT NULL COMMENT 'as its name says',
    `update_time`  timestamp  NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `process_key`  varchar(50)         DEFAULT NULL COMMENT 'process key',
    `is_del`       tinyint(11)             DEFAULT '0',
    `is_all`       tinyint(11)             DEFAULT '0' COMMENT 'it to all',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='process and department join table,used to control access right';

CREATE TABLE if not exists `bpm_process_forward`
(
    `id`                 int(11)      NOT NULL AUTO_INCREMENT,
    `forward_user_id`    varchar(50)            DEFAULT NULL COMMENT 'forwarded user id',
    `Forward_user_name`  varchar(50)           DEFAULT NULL COMMENT 'forwarded user name',
    `processInstance_Id` varchar(64)           DEFAULT NULL COMMENT 'process instance id',
    `node_id`            varchar(64)                            null,
    `create_time`        timestamp             not null default CURRENT_TIMESTAMP COMMENT 'as its name says',
    `create_user_id`     varchar(50)            DEFAULT NULL COMMENT 'as its name says',
    `task_id`            varchar(50)           DEFAULT NULL COMMENT 'taskid',
    `is_read`            int(2)                DEFAULT '0' COMMENT 'is read',
    `is_del`             int(11)               DEFAULT '0',
    `update_time`        timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `process_number`     varchar(50) NOT NULL DEFAULT '' COMMENT 'process number',
    PRIMARY KEY (`id`) USING BTREE,
    KEY `forward_user_id` (`forward_user_id`) USING BTREE,
    KEY `index_forward_user_id_is_read` (`forward_user_id`, `is_read`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='process forward table';


CREATE TABLE if not exists `bpm_process_node_overtime`
(
    `id`          bigint(20) NOT NULL AUTO_INCREMENT,
    `notice_type` int(11)      DEFAULT NULL COMMENT 'notice type 1:email,2:sms 3:app push message',
    `node_name`   varchar(50) DEFAULT NULL COMMENT 'node name',
    `node_key`    varchar(50)  DEFAULT NULL COMMENT 'node id',
    `process_key` varchar(50)  DEFAULT NULL COMMENT 'process key',
    `notice_time` int(11)      DEFAULT NULL COMMENT 'update time',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='process approve overtime notice';

CREATE TABLE if not exists `bpm_process_node_record`
(
    `id`                 bigint(20) NOT NULL AUTO_INCREMENT,
    `processInstance_id` varchar(64)         DEFAULT NULL COMMENT 'process instance id',
    `task_id`            varchar(50)         DEFAULT NULL COMMENT 'taskid',
    `create_time`        timestamp  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='process over time node record';

CREATE TABLE if not exists `bpm_process_node_submit`
(
    `id`                 bigint(20) NOT NULL AUTO_INCREMENT,
    `processInstance_Id` varchar(64)         DEFAULT NULL COMMENT 'process instance id',
    `back_type`          tinyint(11)             DEFAULT NULL COMMENT 'back type',
    `node_key`           varchar(50)         DEFAULT NULL COMMENT 'node key',
    `create_time`        timestamp  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `create_user`        varchar(50)          DEFAULT NULL COMMENT 'creator',
    `state`              tinyint(11)             DEFAULT NULL COMMENT 'state',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='process node submit';

CREATE TABLE if not exists `bpm_process_notice`
(
    `id`          int(11) NOT NULL AUTO_INCREMENT,
    `type`        tinyint(11)      DEFAULT NULL COMMENT 'auto send notice 1.email 2:sms 3:app push',
    `process_key` varchar(50) DEFAULT NULL COMMENT 'process key',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='process notice table';

CREATE TABLE if not exists `bpm_taskconfig`
(
    `id`            bigint(20) NOT NULL AUTO_INCREMENT,
    `proc_def_id_`  varchar(100) DEFAULT NULL COMMENT 'process def id',
    `task_def_key_` varchar(100) DEFAULT NULL COMMENT 'task def key',
    `user_id`       bigint(11)   DEFAULT NULL COMMENT 'user id',
    `number`        int(11)      DEFAULT NULL COMMENT 'number',
    `status`        tinyint(11)      DEFAULT NULL COMMENT 'status',
    `original_type` tinyint(2)       DEFAULT NULL COMMENT 'orginal type',
    PRIMARY KEY (`id`) USING BTREE,
    KEY `BPM_IDX__TASK_CONFIG` (`proc_def_id_`, `task_def_key_`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='task config';



CREATE TABLE if not exists `t_bpm_variable`
(
    `id`                       bigint(20)          NOT NULL AUTO_INCREMENT COMMENT 'id',
    `process_num`              varchar(60)         NOT NULL DEFAULT '' COMMENT 'process number',
    `process_name`             varchar(60)         NOT NULL DEFAULT '' COMMENT 'process name',
    `process_desc`             varchar(255)        NOT NULL DEFAULT '' COMMENT 'process desc',
    `process_start_conditions` text                NOT NULL COMMENT 'process start conditions',
    `bpmn_code`                varchar(60)         NOT NULL DEFAULT '' COMMENT 'bpmn code',
    `is_new_data`              int(11)                      DEFAULT '0' COMMENT 'is new data 0:no 1:yes',
    `remark`                   varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`                   tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:no,1:yes',
    `create_user`              varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `create_time`              timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user`              varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `update_time`              timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE,
    KEY `index_process_num` (`process_num`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='process variable table';

CREATE TABLE if not exists `t_bpm_variable_approve_remind`
(
    `id`          bigint(20)          NOT NULL AUTO_INCREMENT COMMENT 'id',
    `variable_id` bigint(20)          NOT NULL COMMENT 'variable id',
    `element_id`  varchar(60)         NOT NULL DEFAULT '' COMMENT 'element id',
    `content`     text                NOT NULL COMMENT 'approve content',
    `remark`      varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`      tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:no,1:yes',
    `create_user` varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `create_time` timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user` varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `update_time` timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE,
    KEY `variable_id_element_id` (`variable_id`, `element_id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='process remind variable';

CREATE TABLE if not exists `t_bpm_variable_button`
(
    `id`               bigint(20)          NOT NULL AUTO_INCREMENT COMMENT 'id',
    `variable_id`      bigint(20)          NOT NULL COMMENT 'variable id',
    `element_id`       varchar(60)         NOT NULL DEFAULT '' COMMENT 'element id',
    `button_page_type` int(11)             NOT NULL COMMENT 'button,s page type 1:submit page,2:approve page',
    `button_type`      int(11)             NOT NULL COMMENT 'button type see ButtonTypeEnum',
    `button_name`      varchar(60)         NOT NULL DEFAULT '' COMMENT 'button name',
    `remark`           varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`           tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:no,1:yes',
    `create_user`      varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `create_time`      timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user`      varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `update_time`      timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE,
    KEY `variable_id` (`variable_id`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='variable button table';

CREATE TABLE if not exists `t_bpm_variable_message`
(
    `id`           bigint(20)          NOT NULL AUTO_INCREMENT COMMENT 'id',
    `variable_id`  bigint(20)          NOT NULL COMMENT 'variable id',
    `element_id`   varchar(60)         NOT NULL DEFAULT '' COMMENT 'element id',
    `message_type` int(11)             NOT NULL DEFAULT '0' COMMENT 'message type 1:out of node 2:in node',
    `event_type`   int(11)             NOT NULL DEFAULT '0' COMMENT 'event type',
    `content`      text                NOT NULL COMMENT 'content',
    `remark`       varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`       tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:no,1:yes',
    `create_user`  varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `create_time`  timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user`  varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `update_time`  timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE,
    KEY `variable_id_element_id_message_type_event_type` (`variable_id`, `element_id`, `message_type`, `event_type`) USING BTREE,
    KEY `variable_id_message_type_event_type` (`variable_id`, `message_type`, `event_type`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='variable message table';

CREATE TABLE if not exists `t_bpm_variable_multiplayer`
(
    `id`              bigint(20)          NOT NULL AUTO_INCREMENT COMMENT 'id',
    `variable_id`     bigint(20)          NOT NULL COMMENT 'variable id',
    `element_id`      varchar(60)         NOT NULL DEFAULT '' COMMENT 'element id',
    `element_name`    varchar(60)         NOT NULL DEFAULT '' COMMENT 'element name',
    `node_id`        varchar(60)                                             null,
    `collection_name` varchar(60)         NOT NULL DEFAULT '' COMMENT 'collection name',
    `sign_type`       int(11)             NOT NULL COMMENT 'sign type 1: all sign 2:or sign',
    `remark`          varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`          tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:no,1:yes',
    `create_user`     varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `create_time`     timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user`     varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `update_time`     timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE,
    KEY `variable_id` (`variable_id`) USING BTREE,
    KEY `variable_id_element_id` (`variable_id`, `element_id`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='process multiplayer variable table';

CREATE TABLE if not exists `t_bpm_variable_multiplayer_personnel`
(
    `id`                      bigint(20)          NOT NULL AUTO_INCREMENT COMMENT 'id',
    `variable_multiplayer_id` bigint(20)          NOT NULL COMMENT 'variable id',
    `assignee`                varchar(60)         NOT NULL DEFAULT '' COMMENT 'assignee,that is the approver',
    `assignee_name`                varchar(60)         NOT NULL DEFAULT '' COMMENT 'assignee name',
    `undertake_status`        int(11)             NOT NULL COMMENT 'is undertaked(0:no,1:yes)',
    `remark`                  varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`                  tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:no,1:yes',
    `create_user`             varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `create_time`             timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user`             varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `update_time`             timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE,
    KEY `variable_multiplayer_id` (`variable_multiplayer_id`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='multiplayer assignees variable table';

CREATE TABLE if not exists `t_bpm_variable_sequence_flow`
(
    `id`                       bigint(20)          NOT NULL AUTO_INCREMENT COMMENT 'id',
    `variable_id`              bigint(20)          NOT NULL COMMENT 'variable id',
    `element_id`               varchar(60)         NOT NULL DEFAULT '' COMMENT 'element id',
    `element_name`             varchar(60)         NOT NULL DEFAULT '' COMMENT 'element name',
    `element_from_id`          varchar(60)         NOT NULL DEFAULT '' COMMENT 'this node,s prev node',
    `element_to_id`            varchar(60)         NOT NULL DEFAULT '' COMMENT 'this node,s next node',
    `sequence_flow_type`       int(11)             NOT NULL COMMENT 'sequence flow type',
    `sequence_flow_conditions` varchar(100)        NOT NULL DEFAULT '' COMMENT 'sequence flow condition',
    `remark`                   varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`                   tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:no,1:yes',
    `create_user`              varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `create_time`              timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user`              varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `update_time`              timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='sequence flow table';

CREATE TABLE if not exists `t_bpm_variable_sign_up`
(
    `id`                bigint(20)          NOT NULL AUTO_INCREMENT COMMENT 'id',
    `variable_id`       bigint(20)          NOT NULL COMMENT 'variable id',
    `element_id`        varchar(60)         NOT NULL DEFAULT '' COMMENT 'element id',
    `node_id`          varchar(60)                                        null,
    `after_sign_up_way` int(11)             NOT NULL DEFAULT '1' COMMENT 'after sign up way,1:back to current assignee,2 go on',
    `sub_elements`      text                NOT NULL COMMENT 'sub elements stored in json',
    `remark`            varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`            tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:no,1:yes',
    `create_user`       varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `create_time`       timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user`       varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `update_time`       timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE,
    KEY `variable_id` (`variable_id`) USING BTREE,
    KEY `variable_id_element_id` (`variable_id`, `element_id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='process sign up node table';


CREATE TABLE if not exists `t_bpm_variable_sign_up_personnel`
(
    `id`          bigint(20)          NOT NULL AUTO_INCREMENT COMMENT 'id',
    `variable_id` bigint(20)          NOT NULL COMMENT 'variable id',
    `element_id`  varchar(60)         NOT NULL DEFAULT '' COMMENT 'element id',
    `assignee`    varchar(60)         NOT NULL DEFAULT '' COMMENT 'assignee',
    `assignee_name`    varchar(60)   NOT NULL DEFAULT '' COMMENT 'assigneeName',
    `remark`      varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`      tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:no,1:yes',
    `create_user` varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `create_time` timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user` varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `update_time` timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE,
    KEY `variable_id` (`variable_id`) USING BTREE,
    KEY `variable_id_element_id` (`variable_id`, `element_id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='node sign up personnel table';



CREATE TABLE if not exists `t_bpm_variable_single`
(
    `id`                  bigint(20)          NOT NULL AUTO_INCREMENT COMMENT 'id',
    `variable_id`         bigint(20)          NOT NULL COMMENT 'variable id',
    `element_id`          varchar(60)         NOT NULL DEFAULT '' COMMENT 'element id',
    `node_id`            varchar(60)                                      null,
    `element_name`        varchar(60)         NOT NULL DEFAULT '' COMMENT 'element name',
    `assignee_param_name` varchar(60)         NOT NULL DEFAULT '' COMMENT 'variable name',
    `assignee`            varchar(60)         NOT NULL DEFAULT '' COMMENT 'assignee',
    `assignee_name`       varchar(60)         NOT NULL DEFAULT '' COMMENT 'assigneeName',
    `remark`              varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`              tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:no,1:yes',
    `create_user`         varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `create_time`         timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user`         varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `update_time`         timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE,
    KEY `variable_id` (`variable_id`) USING BTREE,
    KEY `variable_id_element_id` (`variable_id`, `element_id`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='node single assignee table';


CREATE TABLE if not exists `t_bpm_variable_view_page_button`
(
    `id`          bigint(20)          NOT NULL AUTO_INCREMENT COMMENT 'id',
    `variable_id` bigint(20)          NOT NULL COMMENT 'variable id',
    `view_type`   int(11)             NOT NULL COMMENT 'view type 1:start user,2:approvers',
    `button_type` int(11)             NOT NULL COMMENT 'button type',
    `button_name` varchar(60)         NOT NULL DEFAULT '' COMMENT 'button name',
    `remark`      varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`      tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:no,1:yes',
    `create_user` varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `create_time` timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user` varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `update_time` timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE,
    KEY `index_variable_id` (`variable_id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='start up view page button params';


CREATE TABLE if not exists `bpm_verify_info`
(
    `id`               bigint(11) NOT NULL AUTO_INCREMENT,
    `run_info_id`      varchar(64)          DEFAULT NULL COMMENT 'process instance id',
    `verify_user_id`   varchar(50)         DEFAULT NULL COMMENT 'approver',
    `verify_user_name` varchar(100)        DEFAULT NULL COMMENT 'approver name',
    `verify_status`    int(1)              DEFAULT NULL COMMENT 'verify status',
    `verify_desc`      varchar(500)       DEFAULT NULL COMMENT 'verify desc',
    `verify_date`      timestamp  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `task_name`        varchar(64)         DEFAULT NULL COMMENT 'tsk name',
    `task_id`          varchar(64)         DEFAULT NULL COMMENT 'task id',
    `task_def_key`     varchar(255)                        null,
    `business_type`    int(1)              DEFAULT NULL COMMENT 'business type',
    `business_id`      varchar(128)        DEFAULT NULL COMMENT 'business id',
    `original_id`      varchar(64)          DEFAULT NULL COMMENT 'orig approver name',
    `process_code`     varchar(64)         DEFAULT NULL COMMENT 'process number',
    PRIMARY KEY (`id`) USING BTREE,
    KEY `BPM_IDX__INFOR` (`business_type`, `business_id`) USING BTREE,
    KEY `process_code_index` (`process_code`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='verify info';



CREATE TABLE if not exists `t_default_template`
(
    `id`          bigint(20) NOT NULL AUTO_INCREMENT,
    `event`       int(11)             DEFAULT NULL COMMENT 'event',
    `template_id` bigint(20)          DEFAULT NULL COMMENT 'template id',
    `is_del`      tinyint(4) NOT NULL DEFAULT '0' COMMENT '（0:no 1:yes',
    `create_time` timestamp  NOT NULL     DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `create_user` varchar(255)        DEFAULT '' COMMENT 'as its name says',
    `update_time` timestamp NOT NULL     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user` varchar(255)        DEFAULT '' COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='default tempalte';



CREATE TABLE if not exists `t_user_email_send`
(
    `id`          int(11)      NOT NULL AUTO_INCREMENT,
    `sender`      varchar(32)  NOT NULL COMMENT 'sender',
    `receiver`    varchar(100) NOT NULL,
    `title`       varchar(255) NOT NULL,
    `content`     text         NOT NULL,
    `create_time` timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `update_time` timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `create_user` varchar(50)  NOT NULL,
    `update_user` varchar(50)  NOT NULL,
    PRIMARY KEY (`id`) USING BTREE,
    KEY `sender` (`receiver`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='user email send';

create table if not exists t_method_replay
(
    id                   int auto_increment
    primary key,
    PROJECT_NAME         varchar(100) null comment 'project name',
    CLASS_NAME           varchar(255) null,
    METHOD_NAME          varchar(255) null,
    PARAM_TYPE           varchar(255) null,
    ARGS                 text         null,
    NOW_TIME             timestamp    null,
    ERROR_MSG            text         null,
    ALREADY_REPLAY_TIMES int          null,
    MAX_REPLAY_TIMES     int          null
    )ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 comment 'method replay records';

create index t_method_replay_NOW_TIME_index
    on t_method_replay (NOW_TIME);


CREATE TABLE if not exists `t_user_entrust`
(
    `id`            int(11)      NOT NULL AUTO_INCREMENT,
    `sender`        varchar(64)      NOT NULL COMMENT 'sender id',
    `receiver_id`   varchar(64)      NOT NULL,
    `receiver_name` varchar(255)          DEFAULT NULL,
    `power_id`      varchar(100) NOT NULL,
    `begin_time`    timestamp    NULL  DEFAULT NULL,
    `end_time`      timestamp    NULL  DEFAULT NULL,
    `create_time`   timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '创建时间',
    `update_time`   timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `create_user`   varchar(50)  NOT NULL,
    `update_user`   varchar(50)  NOT NULL,
    PRIMARY KEY (`id`) USING BTREE,
    UNIQUE KEY `s_r_id` (`sender`, `receiver_id`, `power_id`) USING BTREE,
    KEY `user_id` (`sender`, `power_id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='user entrust info';



CREATE TABLE if not exists `t_user_message_status`
(
    `id`                     int(11)     NOT NULL AUTO_INCREMENT,
    `user_id`                varchar(64)     NOT NULL,
    `message_status`         tinyint(1)  NOT NULL DEFAULT '0' COMMENT 'sms status',
    `mail_status`            tinyint(1)  NOT NULL DEFAULT '0' COMMENT 'email status',
    `not_trouble_time_end`   time             DEFAULT NULL COMMENT 'do not disturb end time',
    `not_trouble_time_begin` timestamp   NULL  DEFAULT NULL COMMENT 'do not disturb begin time',
    `not_trouble`            tinyint(1)  NOT NULL DEFAULT '0' COMMENT 'is do not disturb enabled',
    `shock`                  tinyint(1)  NOT NULL DEFAULT '0' COMMENT 'should shock',
    `sound`                  tinyint(1)  NOT NULL DEFAULT '0' COMMENT 'is in silent mode',
    `open_phone`             tinyint(1)  NOT NULL DEFAULT '0' COMMENT '',
    `create_time`            timestamp   NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `update_time`            timestamp   NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `create_user`            varchar(50) NOT NULL,
    `update_user`            varchar(50) NOT NULL,
    PRIMARY KEY (`id`) USING BTREE,
    KEY `user_id` (`user_id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='user receive message table';

CREATE TABLE if not exists `t_bpmn_node_button_conf`
(
    `id`               bigint(20)          NOT NULL AUTO_INCREMENT COMMENT 'id',
    `bpmn_node_id`     bigint(20)          NOT NULL COMMENT 'node id',
    `button_page_type` int(11)             NOT NULL COMMENT 'button type 1:start page;2:approval page',
    `button_type`      int(11)             NOT NULL COMMENT 'button type',
    `button_name`      varchar(60)                  DEFAULT '' COMMENT 'button name',
    `remark`           varchar(255)        NOT NULL DEFAULT '' COMMENT 'remark',
    `is_del`           tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '0:no 1:yes',
    `create_user`      varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `create_time`      timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'asits name says',
    `update_user`      varchar(50)                  DEFAULT '' COMMENT 'as its name says',
    `update_time`      timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='node button conf';

CREATE TABLE if not exists `bpm_business_process`
(
    `id`               bigint(20)  NOT NULL AUTO_INCREMENT COMMENT 'id',
    `PROCESSINESS_KEY` varchar(64)  DEFAULT NULL,
    `BUSINESS_ID`      varchar(64) NOT NULL COMMENT 'business id',
    `BUSINESS_NUMBER`  varchar(64)  DEFAULT NULL COMMENT 'process number',
    `ENTRY_ID`         varchar(64)  DEFAULT NULL,
    `VERSION`          varchar(30)   DEFAULT NULL COMMENT 'version',
    `CREATE_TIME`      timestamp     NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `UPDATE_TIME`      timestamp      NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    `description`      varchar(100) DEFAULT NULL COMMENT 'title',
    `process_state`    int(1)       DEFAULT NULL COMMENT 'process state 1:approving 2:approved 3:invalid 6:rejected',
    `create_user`      varchar(64)  DEFAULT NULL,
    `process_digest`   text COMMENT 'process digest',
    `is_del`           tinyint(1)   DEFAULT '0' COMMENT '0: no 1: yes）',
    `data_source_id`   bigint(10)   DEFAULT NULL COMMENT 'data source id',
    `PROC_INST_ID_`    varchar(64)  DEFAULT '' COMMENT 'process instance id',
    `back_user_id`     varchar(64)      DEFAULT NULL COMMENT 'back to user id',
    user_name           varchar(255)           null,
    is_out_side_process tinyint     default 0  null comment 'is it an outside process,0 no,1 yes',
    is_lowcode_flow     tinyint     default 0  null comment '是否是低代码工作流0,否,1是',
    PRIMARY KEY (`id`) USING BTREE,
    KEY `PROC_INST_ID_index` (`PROC_INST_ID_`) USING BTREE,
    KEY `process_entry_id` (`ENTRY_ID`) USING BTREE,
    KEY `process_key_index` (`PROCESSINESS_KEY`) USING BTREE,
    KEY `process_number_index` (`BUSINESS_NUMBER`) USING BTREE,
    KEY `process_state_index` (`process_state`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='process and business association table';




CREATE TABLE if not exists `bpm_process_name_relevancy`
(
    `id`              bigint(20) NOT NULL AUTO_INCREMENT,
    `process_name_id` bigint(20)      DEFAULT NULL COMMENT 'process name id',
    `process_key`     varchar(50)     DEFAULT NULL COMMENT 'process key',
    `is_del`          int(11)         DEFAULT '0',
    `create_time`     timestamp  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`id`) USING BTREE,
    KEY `process_key_index` (`process_key`) USING BTREE,
    KEY `process_name_id_index` (`process_name_id`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='process advanced search table';



CREATE TABLE if not exists `t_bpmn_node_personnel_conf`
(
    `id`           int(11) NOT NULL AUTO_INCREMENT COMMENT 'd',
    `bpmn_node_id` int(11) NOT NULL COMMENT '节点id',
    `sign_type`    tinyint(11) DEFAULT NULL COMMENT 'sign type 1: all sign 2:or sign',
    `remark`       varchar(100) DEFAULT NULL COMMENT 'remark',
    `is_del`       tinyint(1) DEFAULT NULL COMMENT '0:no,1:yes',
    `create_user`  varchar(50)  DEFAULT NULL COMMENT 'as its name says',
    `create_time`  timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user`  varchar(50)  DEFAULT NULL COMMENT 'as its name says',
    `update_time`  timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='node person conf table';

CREATE TABLE if not exists `t_bpmn_node_personnel_empl_conf`
(
    `id`                    int(11)    NOT NULL AUTO_INCREMENT COMMENT 'id',
    `bpmn_node_personne_id` int(11)    NOT NULL COMMENT 'personnel id',
    `empl_id`               varchar(50) NOT NULL COMMENT 'approver id',
    `empl_name`             varchar(50) null comment 'approver name',
    `remark`                varchar(100) DEFAULT NULL COMMENT 'remark',
    `is_del`                tinyint(255) DEFAULT NULL COMMENT '0:no,1:yes',
    `create_user`           varchar(30)  DEFAULT NULL COMMENT 'as its name says',
    `create_time`           timestamp     NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user`           varchar(30)  DEFAULT NULL COMMENT 'as its name says',
    `update_time`           timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='node assignee employee conf';

CREATE TABLE IF NOT EXISTS `bpm_process_operation`
(
    `id`           bigint(20) NOT NULL AUTO_INCREMENT,
    `process_key`  varchar(50) DEFAULT NULL COMMENT 'process key',
    `process_node` varchar(50) DEFAULT NULL COMMENT 'process node id',
    `type`         int(2)      DEFAULT NULL COMMENT '1:batch submit 2:entrust',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='process operation table';

CREATE TABLE if not exists `bpm_process_node_back`
(
    `id`          bigint(20) NOT NULL AUTO_INCREMENT,
    `node_key`    varchar(50)  DEFAULT NULL COMMENT 'node key',
    node_id     bigint       null comment '节点id',
    `back_type`   int(11)      DEFAULT NULL COMMENT 'back type',
    `process_key` varchar(100) DEFAULT NULL COMMENT 'process key',
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4 COMMENT ='process node back table';


CREATE TABLE if not exists `bpm_process_name`
(
    `id`           bigint(20) NOT NULL AUTO_INCREMENT,
    `process_name` varchar(50)     DEFAULT NULL COMMENT 'process name',
    `is_del`       int(11)         DEFAULT '0',
    `create_time`  timestamp  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`id`) USING BTREE
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4 COMMENT ='process advanced search table';

CREATE TABLE if not exists `t_user_message`
(
    id          bigint auto_increment
    primary key,
    user_id     varchar(64)          null comment '用户id',
    title       varchar(50)  null comment '标题',
    content     varchar(255) null comment '消息内容',
    url         varchar(255) null comment '发送url',
    node        varchar(50)  null comment '发送节点id',
    params      varchar(255) null comment '发送类型',
    is_read     tinyint(255) null comment '0为未读 1为已读',
    is_del         tinyint(255) null comment '0为未删除 1为已删除',
    create_time datetime     null,
    update_time datetime     null,
    create_user varchar(50)  null,
    update_user varchar(50)  null,
    app_url     varchar(255) null comment 'appurl',
    source      int          null
    ) ENGINE = InnoDB
    DEFAULT CHARSET = utf8mb4;

CREATE TABLE IF NOT EXISTS `t_op_log`
(
    `id`             bigint(20) NOT NULL AUTO_INCREMENT,
    `msg_id`         varchar(64)  DEFAULT NULL COMMENT ' msg id',
    `op_flag`        tinyint(255) DEFAULT NULL COMMENT '0=success, 1=fail, 2=business exception',
    `op_user_no`     varchar(50)  DEFAULT NULL COMMENT 'user no',
    `op_user_name`   varchar(50)  DEFAULT NULL COMMENT 'user name',
    `op_method`      varchar(255) DEFAULT NULL COMMENT 'op method',
    `op_time`        timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'op time',
    `op_use_time`    bigint(20)   DEFAULT NULL COMMENT 'time cost',
    `op_param`       text COMMENT 'op params',
    `op_result`      text COMMENT 'op result',
    `system_type`    tinyint(255) DEFAULT NULL COMMENT 'operation system type，iOS，Android，1=PC',
    `app_version`    varchar(50)  DEFAULT NULL COMMENT 'app version',
    `hardware`       varchar(50)  DEFAULT NULL COMMENT 'hardware info',
    `system_version` varchar(50)  DEFAULT NULL COMMENT 'app version',
    `remark`         varchar(255) DEFAULT NULL COMMENT 'remark',
    PRIMARY KEY (`id`)
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4
    COLLATE = utf8mb4_general_ci;

CREATE TABLE IF NOT EXISTS  `third_party_account_apply`
(
    `id`                 int(11) NOT NULL AUTO_INCREMENT,
    `account_type`       tinyint(4)   DEFAULT NULL COMMENT 'account type',
    `account_owner_name` varchar(50)  DEFAULT NULL COMMENT 'account owner',
    `remark`             varchar(200) DEFAULT NULL COMMENT 'remark',
    PRIMARY KEY (`id`)
    ) ENGINE = InnoDB
    AUTO_INCREMENT = 1
    DEFAULT CHARSET = utf8mb4
    COLLATE = utf8mb4_general_ci COMMENT ='a third party account apply demo';


CREATE TABLE IF NOT EXISTS `t_bpmn_node_out_side_access_conf` (
                                                                  `id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT 'auto increment id',
    `bpmn_node_id` bigint(20) NOT NULL COMMENT 'bpmn_node_id',
    `node_mark` varchar(50) DEFAULT NULL COMMENT 'node mark',
    `sign_type` int(11) DEFAULT NULL COMMENT 'sign type 1 for all sign 2 for or sign',
    `remark` varchar(255) DEFAULT NULL COMMENT 'remark',
    `is_del` int(11) NOT NULL DEFAULT '0' COMMENT '0 for normal 1 for delete',
    `create_user` varchar(50) DEFAULT NULL COMMENT 'as its name says',
    `create_time` timestamp not null default CURRENT_TIMESTAMP  COMMENT 'as its name says',
    `update_user` varchar(50) DEFAULT NULL COMMENT 'as its name says',
    `update_time`  timestamp not null default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='node conf for outside access';

CREATE TABLE IF NOT EXISTS  bpm_process_app_application
(
    id               int auto_increment comment 'Primary key'
    primary key,
    business_code    varchar(50)                       null comment 'Business code, typically empty for main applications',
    process_name     varchar(50)                       null comment 'Application name',
    apply_type       int                                null comment 'Application type (1: Process, 2: Application, 3: Parent Application)',
    permissions_code varchar(50)                       null,
    pc_icon          varchar(500)                       null comment 'PC icon URL or path',
    effective_source varchar(500)                       null comment 'Mobile platform icon URL or identifier (based on the field name)',
    is_son           int                                null comment 'Whether it is a child application (0: No, 1: Yes)',
    look_url         varchar(500)                       null comment 'URL for viewing the application',
    submit_url       varchar(500)                       null comment 'URL for submitting the application',
    condition_url    varchar(500)                       null comment 'URL for accessing conditions or rules related to the application',
    parent_id        int                                null comment 'Parent application ID (if this is a child application)',
    application_url  varchar(500)                       null comment 'Main URL of the application',
    route            varchar(500)                       null comment 'Application route or path',
    process_key      varchar(50)                       null comment 'Process key or identifier',
    create_time      timestamp  not null default CURRENT_TIMESTAMP  comment 'Creation timestamp',
    update_time      timestamp not null default CURRENT_TIMESTAMP  on update CURRENT_TIMESTAMP comment 'Last update timestamp',
    is_del           tinyint      default 0                not null,
    create_user_id   varchar(64)                                null,
    update_user      varchar(255)                       null,
    is_all           tinyint  default 0                 null,
    state            tinyint  default 1                 null,
    sort             int                                null,
    source           varchar(255)                       null
    )comment 'BPM Process Application Table';


CREATE TABLE IF NOT EXISTS `bpm_process_app_data` (
                                                      `id` BIGINT AUTO_INCREMENT COMMENT 'Primary key',
                                                      `process_key` VARCHAR(50) COMMENT 'Process key',
    `process_name` VARCHAR(50) COMMENT 'Process name',
    `state` INT COMMENT 'Is online (0 for no, 1 for yes)',
    `route` VARCHAR(500) COMMENT 'APP route',
    `sort` INT COMMENT 'Sort order',
    `source` VARCHAR(500) COMMENT 'Pic source route',
    `is_all` tinyint COMMENT 'Is for all (0 or 1)',
    `version_id` BIGINT COMMENT 'Version ID',
    `application_id` BIGINT COMMENT 'Application ID',
    `type` INT COMMENT 'Type (1 for version app, 2 for app data)',
    PRIMARY KEY (`id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='App Online Process Data Table';

CREATE TABLE IF NOT EXISTS `bpm_process_application_type` (
                                                              `id` BIGINT AUTO_INCREMENT COMMENT 'Primary key',
                                                              `application_id` BIGINT COMMENT 'Application ID',
                                                              `category_id` BIGINT COMMENT 'Category ID',
                                                              `is_del` INT COMMENT 'Deletion flag (0 for not deleted, 1 for deleted)',
                                                              `sort` INT COMMENT 'Sort order',
                                                              `state` INT COMMENT 'Is frequently used (0 for no, 1 for yes)',
                                                              `history_id` BIGINT COMMENT 'History ID',
                                                              `visble_state` INT COMMENT 'Visibility state (0 for hidden, 1 for visible)',
                                                              `create_time` timestamp not null default current_timestamp COMMENT 'Creation time',
                                                              `common_use_state` INT COMMENT 'Common use state',
                                                              PRIMARY KEY (`id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='BPM Process Application Type Table';

CREATE TABLE IF NOT EXISTS `bpm_process_category` (
                                                      `id` BIGINT AUTO_INCREMENT COMMENT 'Primary key',
                                                      `process_type_name` VARCHAR(255) COMMENT 'Process type name',
    `is_del` TINYINT(1) COMMENT 'Deletion flag (0 for not deleted, 1 for deleted)',
    `state` INT COMMENT 'State of the category (specific meaning depends on application logic)',
    `sort` INT COMMENT 'Sort order',
    `is_app` TINYINT(1) COMMENT 'Is for app (0 for no, 1 for yes)',
    `entrance` VARCHAR(255) COMMENT 'Entrance (specific meaning depends on application logic)',
    PRIMARY KEY (`id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='BPM Process Category Table';

CREATE TABLE IF NOT EXISTS `bpm_process_permissions` (
                                                         `id` BIGINT AUTO_INCREMENT COMMENT 'Primary key',
                                                         `user_id` varchar(64) COMMENT 'User ID',
    `dep_id` BIGINT COMMENT 'Department ID',
    `permissions_type` INT COMMENT 'Permission type (1 for view, 2 for create, 3 for monitor)',
    `create_user` varchar(64) COMMENT 'Create user ID',
    `create_time` timestamp not null default current_timestamp COMMENT 'Create time',
    `process_key` VARCHAR(50) COMMENT 'Process key',
    `office_id` BIGINT COMMENT 'Office ID',
    PRIMARY KEY (`id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='process permission';


CREATE TABLE IF NOT EXISTS  `t_out_side_bpm_access_business` (
                                                                 `id` BIGINT AUTO_INCREMENT,
                                                                 `business_party_id` BIGINT NOT NULL,
                                                                 `bpmn_conf_id` BIGINT NOT NULL,
                                                                 `form_code` VARCHAR(50) DEFAULT NULL,
    `process_number` VARCHAR(50) DEFAULT NULL,
    `form_data_pc` LONGTEXT,
    `form_data_app` LONGTEXT,
    `template_mark` VARCHAR(50) DEFAULT NULL,
    `start_username` VARCHAR(50) DEFAULT NULL,
    `remark` TEXT,
    `is_del` TINYINT DEFAULT 0,
    `create_user` VARCHAR(50) DEFAULT NULL,
    `create_time` timestamp DEFAULT CURRENT_TIMESTAMP,
    `update_user` VARCHAR(50) DEFAULT NULL,
    `update_time` timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`id`),
    KEY `idx_bpm_conf_id` (`bpmn_conf_id`),
    KEY `idx_business_party_id` (`business_party_id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE  IF NOT EXISTS  `t_out_side_bpm_admin_personnel` (
                                                                  `id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT 'Auto increment ID',
    `business_party_id` bigint(20) DEFAULT NULL COMMENT 'Business party main table ID',
    `type` int(11) DEFAULT NULL COMMENT 'Administrator type: 1-Process administrator, 2-Application administrator, 3-Interface administrator',
    `employee_id` varchar(64) DEFAULT NULL COMMENT 'Administrator ID (Employee ID)',
    `employee_name` varchar(64) DEFAULT NULL COMMENT 'Administrator name (Employee name)',
    `remark` varchar(255) DEFAULT NULL COMMENT 'Remark',
    `is_del` int(11) DEFAULT NULL COMMENT 'Deletion flag: 0 for normal, 1 for deleted',
    `create_user` varchar(50) DEFAULT NULL COMMENT 'Creator user',
    `create_time` timestamp DEFAULT CURRENT_TIMESTAMP COMMENT 'Creation time',
    `update_user` varchar(50) DEFAULT NULL COMMENT 'Updater user',
    `update_time` timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Update time',
    PRIMARY KEY (`id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Workflow External Service - Business Party Administrator Table';

CREATE TABLE IF NOT EXISTS  `t_out_side_bpm_business_party` (
                                                                `id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT 'Auto incr id',
    `business_party_mark` varchar(50) DEFAULT NULL COMMENT 'Business party mark',
    `name` varchar(255) DEFAULT NULL COMMENT 'Business party name',
    `type` tinyint(11) DEFAULT NULL COMMENT 'Business type: 1 for embedded, 2 for API access',
    `remark` varchar(255) DEFAULT NULL COMMENT 'Remark',
    `is_del` tinyint(11) DEFAULT 0 COMMENT 'Deletion flag: 0 for normal, 1 for deleted',
    `create_user` varchar(50) DEFAULT NULL COMMENT 'Creator user',
    `create_time` timestamp DEFAULT CURRENT_TIMESTAMP COMMENT 'Creation time',
    `update_user` varchar(50) DEFAULT NULL COMMENT 'Updater user',
    `update_time` timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  COMMENT 'Update time',
    PRIMARY KEY (`id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Table for storing business party information in the external BPM system';

CREATE TABLE IF NOT EXISTS  t_out_side_bpm_callback_url_conf
(
    id                    bigint auto_increment comment 'auto increment id'
    primary key,
    business_party_id     bigint       null comment 'business party id',
    bpmn_conf_id          bigint       null comment 'bpmn confi id',
    form_code         varchar(64) null comment 'formcode',
    bpm_conf_callback_url varchar(500) null comment 'conf callback url',
    bpm_flow_callback_url varchar(500) null comment 'process flow call back url',
    api_client_id         varchar(100) null comment 'appId',
    api_client_secret    varchar(100) null comment 'appSecret',
    status                tinyint default 0 comment '0 for enable,1 for disable',
    create_user           varchar(50) null comment 'as its name says',
    update_user           varchar(50) null,
    remark                varchar(50)         null comment 'remark',
    is_del                tinyint     default 0 comment '0 for normal,1 for delete',
    create_time           timestamp DEFAULT CURRENT_TIMESTAMP comment 'as its name says',
    update_time           timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP comment 'as its name says'
    ) comment 'business party callback url conf';

-- ----------------------------
-- Table structure for t_out_side_bpm_approve_template
-- ----------------------------
DROP TABLE IF EXISTS `t_out_side_bpm_approve_template`;
CREATE TABLE IF NOT EXISTS  `t_out_side_bpm_approve_template` (
                                                                  `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT 'auto increment id',
                                                                  `business_party_id` BIGINT NULL COMMENT '业务方项目 Id',
                                                                  `application_id` INT NULL COMMENT '项目下业务表单 id',
                                                                  `approve_type_id` INT NULL COMMENT '审批人类型 id',
                                                                  `approve_type_name` VARCHAR(50) NULL COMMENT '审批人类型名称',
    `api_client_id` VARCHAR(50) NULL COMMENT 'api_client_id',
    `api_client_secret` VARCHAR(50) NULL COMMENT 'api_client_secret',
    `api_token` VARCHAR(50) NULL COMMENT 'api_token',
    `api_url` VARCHAR(50) NULL COMMENT 'api_url',
    `remark` varchar(255) NULL COMMENT 'remark',
    `is_del` TINYINT default  0 COMMENT '0 for normal, 1 for delete',
    `create_user` VARCHAR(50) NULL COMMENT 'as its name says',
    `create_time` timestamp DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user` VARCHAR(50) NULL COMMENT 'as its name says',
    `update_time` timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    `create_user_id` varchar(64) NULL COMMENT 'as its name says',
    PRIMARY KEY (`id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='outside access process,approve template config';

-- ----------------------------
-- Table structure for t_out_side_bpm_conditions_template
-- ----------------------------
DROP TABLE IF EXISTS `t_out_side_bpm_conditions_template`;
CREATE TABLE IF NOT EXISTS  `t_out_side_bpm_conditions_template` (
                                                                     `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT 'auto increment id',
                                                                     `business_party_id` BIGINT NULL COMMENT 'business party Id',
                                                                     `template_mark` VARCHAR(50) NULL COMMENT 'template mark',
    `template_name` VARCHAR(50) NULL COMMENT 'template name',
    `application_id` INT NULL COMMENT 'application id',
    `remark` varchar(255) NULL COMMENT 'remark',
    `is_del` TINYINT default  0 COMMENT '0 for normal, 1 for delete',
    `create_user` VARCHAR(50) NULL COMMENT 'as its name says',
    `create_time` timestamp DEFAULT CURRENT_TIMESTAMP COMMENT 'as its name says',
    `update_user` VARCHAR(50) NULL COMMENT 'as its name says',
    `update_time` timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    `create_user_id` varchar(64) NULL COMMENT 'as its name says',
    PRIMARY KEY (`id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='outside access process,condition template config';

CREATE TABLE IF NOT EXISTS  `t_out_side_bpmn_node_conditions_conf` (
                                                                       `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT 'auto increment id',
                                                                       `bpmn_node_id` BIGINT NULL COMMENT 'node id',
                                                                       `out_side_id` VARCHAR(50) NULL COMMENT 'outSide Conditions id',
    `remark` varchar(255) NULL COMMENT 'remark',
    `is_del` INT NULL COMMENT '0 for normal,1 for delete',
    `create_user` VARCHAR(50) NULL COMMENT 'as its name says',
    `create_time` timestamp DEFAULT CURRENT_TIMESTAMP  COMMENT 'as its name says',
    `update_user` VARCHAR(50) NULL COMMENT 'as its name says',
    `update_time` timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'as its name says',
    PRIMARY KEY (`id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='outside access process,business party,s conditions configs';

CREATE TABLE IF NOT EXISTS  `t_out_side_bpm_call_back_record` (
                                                                  `id` INT NOT NULL AUTO_INCREMENT COMMENT 'auto increment id',
                                                                  `process_number` VARCHAR(50) NULL COMMENT 'process number',
    `status` tinyint NULL COMMENT 'push status 0 for success,1 for fail',
    `retry_times` tinyint NULL COMMENT 'retry times',
    `button_operation_type` tinyint NULL COMMENT 'operation type (see MsgProcessEventEnum)',
    `call_back_type_name` VARCHAR(255) NULL COMMENT 'call back type name (see CallbackTypeEnum)',
    `business_id` BIGINT NULL COMMENT 'business id',
    `form_code` VARCHAR(50) NULL COMMENT 'form code',
    `is_del` tinyint default 0 COMMENT '0 for normal,1 for delete',
    `create_user` VARCHAR(50) NULL COMMENT 'create user',
    `create_time` timestamp DEFAULT CURRENT_TIMESTAMP COMMENT 'create time',
    `update_user` VARCHAR(50) NULL COMMENT 'update user',
    `update_time` timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'update time',
    PRIMARY KEY (`id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Table for storing callback records';

CREATE TABLE IF NOT EXISTS  `t_quick_entry` (
                                                `id` INT AUTO_INCREMENT PRIMARY KEY,
                                                `title` VARCHAR(100) NOT NULL,
    `effective_source` VARCHAR(255),
    `is_del` TINYINT DEFAULT 0,
    `route` VARCHAR(500) NOT NULL,
    `sort` tinyint DEFAULT 0,
    `create_time` timestamp DEFAULT CURRENT_TIMESTAMP,
    `status` TINYINT NOT NULL DEFAULT 0,
    `variable_url_flag` TINYINT NOT NULL DEFAULT 0,
    INDEX `idx_route` (`route`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE  IF NOT EXISTS `t_quick_entry_type` (
                                                     `id` BIGINT AUTO_INCREMENT PRIMARY KEY,
                                                     `quick_entry_id` BIGINT NOT NULL,
                                                     `type` INT NOT NULL COMMENT 'Type 1 for PC and 2 for app',
                                                     `is_del` TINYINT DEFAULT 0 COMMENT 'Delete flag',
                                                     `create_time` timestamp DEFAULT CURRENT_TIMESTAMP,
                                                     `type_name` VARCHAR(255) NOT NULL COMMENT 'Type name',
    INDEX `idx_quick_entry_id` (`quick_entry_id`)

    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='quick entry type config';

CREATE TABLE IF NOT EXISTS  `t_sys_version` (
                                                `id` BIGINT AUTO_INCREMENT PRIMARY KEY,
                                                `create_time` timestamp DEFAULT CURRENT_TIMESTAMP,
                                                `update_time` timestamp not null default current_timestamp on update current_timestamp,
                                                `is_del` TINYINT DEFAULT 0 COMMENT '0 for normal, 1 for deleted',
                                                `version` VARCHAR(100) NOT NULL COMMENT 'Version',
    `description` varchar(255) COMMENT 'Version description',
    `index` INT COMMENT 'Index',
    `is_force` TINYINT COMMENT 'Force update 0 for no, 1 for yes',
    `android_url` VARCHAR(500) COMMENT 'Android download URL',
    `ios_url` VARCHAR(500) COMMENT 'iOS download URL',
    `create_user` VARCHAR(50) COMMENT 'Create user',
    `update_user` VARCHAR(50) COMMENT 'Update user',
    `is_hide` TINYINT COMMENT '0 for not hide and 1 for hide',
    `download_code` VARCHAR(255) COMMENT 'Download code',
    `effective_time` timestamp COMMENT 'Effective time',
    INDEX `idx_version` (`version`) -- Optional index for improved performance on `version` queries
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='sys version control';

CREATE TABLE IF NOT EXISTS `t_bpmn_node_role_conf` (
                                                       `id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT 'auto incr id',
    `bpmn_node_id` BIGINT(20) NOT NULL COMMENT 'node id',
    `role_id` varchar(64) NOT NULL COMMENT 'role id',
    `role_name` varchar(64) NOT NULL COMMENT 'role name',
    `sign_type` INT(11) NOT NULL COMMENT 'sign type 1 all sign,2 or sign',
    `remark` VARCHAR(255) DEFAULT NULL COMMENT 'remark',
    `is_del` TINYINT(1) NOT NULL DEFAULT '0' COMMENT '0:normal,1:deleted',
    `create_user` VARCHAR(50) NOT NULL COMMENT 'create user',
    `create_time` timestamp DEFAULT CURRENT_TIMESTAMP COMMENT 'create time',
    `update_user` VARCHAR(50) DEFAULT NULL COMMENT 'update user',
    `update_time` timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'update time',
    PRIMARY KEY (`id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='specified role approver configs';

create table if NOT EXISTS t_bpmn_node_role_outside_emp_conf
(
    id          int auto_increment comment 'auto incr id'
    primary key,
    node_id     bigint                             null comment 'foreign key for connect with t_bpmn_node_role_conf',
    empl_id     varchar(64)                             null comment 'assignee id',
    empl_name   varchar(50)                        null comment 'assignee''s name',
    create_user varchar(50) charset utf8           null,
    `create_time` timestamp DEFAULT CURRENT_TIMESTAMP COMMENT 'create time',
    update_user varchar(255) charset utf8          null,
    column_8    int                                null,
    `update_time` timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'update time',
    `is_del` TINYINT(1) NOT NULL DEFAULT '0' COMMENT '0:normal,1:deleted'
    )
    comment 'approver info for a specified outsie business party''s specified role';


CREATE TABLE IF NOT EXISTS  `t_bpmn_node_loop_conf` (
                                                        `id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT 'auto incr id',
    `bpmn_node_id` BIGINT(20) NULL COMMENT 'node id',
    `loop_end_type` INT(11) NULL COMMENT 'loop type 1 for organizational line; 2 for reporting line',
    `loop_number_plies` INT(11) NULL COMMENT 'how many levels',
    `loop_end_person` VARCHAR(50) NULL COMMENT 'end user',
    `noparticipating_staff_ids` VARCHAR(255) NULL COMMENT 'end staff ids',
    `loop_end_grade` INT(11) NULL COMMENT 'end level',
    `remark` VARCHAR(255) NULL COMMENT 'remark',
    `is_del` TINYINT(1) NOT NULL DEFAULT '0' COMMENT '0:not deleted,1:deleted',
    `create_user` VARCHAR(50) NULL COMMENT 'create user',
    `create_time` timestamp DEFAULT CURRENT_TIMESTAMP COMMENT 'create time',
    `update_user` VARCHAR(50) NULL COMMENT 'update user',
    `update_time` timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'update time',
    PRIMARY KEY (`id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='loop approvement config';

CREATE TABLE IF NOT EXISTS `t_bpmn_node_assign_level_conf` (
                                                               `id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT 'auto incr id',
    `bpmn_node_id` BIGINT(20) NULL COMMENT 'node id',
    `assign_level_type` tinyint(11) NULL COMMENT 'level type 1 for organizational line; 2 for staff orginal head; 3 for reporting line',
    `assign_level_grade` tinyint(11) NULL COMMENT 'level',
    `remark` VARCHAR(255) NULL COMMENT 'remark',
    `is_del` TINYINT(1) NOT NULL DEFAULT '0' COMMENT '0:not deleted,1:deleted',
    `create_user` VARCHAR(255) NULL COMMENT 'create user',
    `create_time` timestamp DEFAULT CURRENT_TIMESTAMP  COMMENT 'create time',
    `update_user` VARCHAR(255) NULL COMMENT 'update user',
    `update_time` timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'update time',
    PRIMARY KEY (`id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='specified level approvement config';

CREATE TABLE `t_bpmn_node_hrbp_conf` (
                                         `id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT 'auto incr id',
                                         `bpmn_node_id` BIGINT(20) NULL COMMENT 'node id',
                                         `hrbp_conf_type` INT(11) NULL COMMENT 'hrbp type 1-hrbp,2-hrbp leader,this is only for extensibility purpose,if your system do not have hrbp leader,you can ignore this field if your system have other concept,for example hrbp manager,you can use this field to store hrbp manager(eg 3 for hrbp manager)',
                                         `remark` VARCHAR(255) NULL COMMENT 'remark',
                                         `is_del` INT(11) NULL COMMENT '0 for normal,1 for deleted',
                                         `create_user` VARCHAR(255) NULL COMMENT 'create user',
                                         `create_time` DATETIME NULL COMMENT 'create time',
                                         `update_user` VARCHAR(255) NULL COMMENT 'update user',
                                         `update_time` DATETIME NULL COMMENT 'update time',
                                         PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='hrpb config entity';

create table t_department
(
    id          int auto_increment comment 'Primary key'
        primary key,
    name        varchar(255) null comment 'Name',
    short_name  varchar(255) null comment 'Short name',
    parent_id   int          null comment 'Parent ID',
    path        varchar(255) null comment 'Path',
    level       int          null comment 'Department level',
    leader_id   bigint       null,
    sort        int          null comment 'Sort order',
    is_del      tinyint      null comment 'Is deleted (0 for no, 1 for yes)',
    is_hide     tinyint      null comment 'Is hidden (0 for show, 1 for hide)',
    create_user varchar(255) null comment 'Create user',
    update_user varchar(255) null comment 'Update user',
    create_time datetime     null comment 'Creation time',
    update_time datetime     null comment 'Update time'
) comment 'department info';



DROP TABLE IF EXISTS `t_user`;
create table if not exists t_user
(
    id             int auto_increment
    primary key,
    user_name      varchar(255)      null,
    mobile         varchar(50)       null comment 'user''s mobile phone number',
    email          varchar(50)       null comment 'user''s email address',
    leader_id      bigint            null comment 'emp direct leader id',
    hrbp_id        bigint            null comment '用户的hrb的id,这里仅仅是用作选择hrbp审批时展示使用,实际上有的的公司组织架构里每个员工都有一个hrbp,有的则是hrbp挂在部门下面.具体根据公司业务而定',
    mobile_is_show tinyint default 0 null comment '是否展示用户手机号,如果用户手机号不展示时发送流程短信通知时也不应当通知给他.当然有的公司设置必须发送短信.这个根据公司业务而定,这里只是展示可以做很多人性化的定制设置',
    department_id  bigint            null comment '部门id',
    path           varchar(1000)     null comment '员工组织线path,用于层层审批流程展示,有些公司的表员工的组织链并非这样的,这里只是展示用,具体根据公司业务而定只要能根据员工的id找到他的上级,上级的上线,上级的上级的上线的上线即可.当然如果没有这样的组织架构关系,不用这种审批规则即可',
    is_del         tinyint default 0 null comment '0,正常1,删除',
    head_img       varchar(3000)     null
    );

-- ----------------------------
-- Records of t_user
-- ----------------------------
INSERT INTO `t_user`(`id`, `user_name`) VALUES (1, '张三');
INSERT INTO `t_user`(`id`, `user_name`) VALUES (2, '李四');
INSERT INTO `t_user`(`id`, `user_name`) VALUES (3, '王五');
INSERT INTO `t_user`(`id`, `user_name`) VALUES (4, '菜六');
INSERT INTO `t_user`(`id`, `user_name`) VALUES (5, '牛七');
INSERT INTO `t_user`(`id`, `user_name`) VALUES (6, '马八');
INSERT INTO `t_user`(`id`, `user_name`) VALUES (7, '李九');
INSERT INTO `t_user`(`id`, `user_name`) VALUES (8, '周十');
INSERT INTO `t_user`(`id`, `user_name`) VALUES (9, '肖十一');
INSERT INTO `t_user`(`id`, `user_name`) VALUES (10, '令狐冲');
INSERT INTO `t_user`(`id`, `user_name`) VALUES (11, '风清扬');
INSERT INTO `t_user`(`id`, `user_name`) VALUES (12, '刘正风');
INSERT INTO `t_user`(`id`, `user_name`) VALUES (13, '岳不群');
INSERT INTO `t_user`(`id`, `user_name`) VALUES (14, '宁中则');
INSERT INTO `t_user`(`id`, `user_name`) VALUES (15, '桃谷六仙');
INSERT INTO `t_user`(`id`, `user_name`) VALUES (16, '不介和尚');
INSERT INTO `t_user`(`id`, `user_name`) VALUES (17, '丁一师太');
INSERT INTO `t_user`(`id`, `user_name`) VALUES (18, '依林师妹');
INSERT INTO `t_user`(`id`, `user_name`) VALUES (19, '邱灵珊');
INSERT INTO `t_user`(`id`, `user_name`) VALUES (20, '任盈盈');
INSERT INTO `t_user`(`id`, `user_name`) VALUES (1001, 'test');


DROP TABLE IF EXISTS `t_role`;
CREATE TABLE `t_role`  (
                           `id` int(11) NOT NULL AUTO_INCREMENT,
                           `role_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                           PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 9 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of t_role
-- ----------------------------
INSERT INTO `t_role` VALUES (1, '审核管理员');
INSERT INTO `t_role` VALUES (2, '招商事业部');
INSERT INTO `t_role` VALUES (3, '互联网部门');
INSERT INTO `t_role` VALUES (4, '销售部');
INSERT INTO `t_role` VALUES (5, '战区一');
INSERT INTO `t_role` VALUES (6, '战区二');
INSERT INTO `t_role` VALUES (7, 'JAVA开发');
INSERT INTO `t_role` VALUES (8, '测试审批角色');



-- ----------------------------
-- Table structure for t_biz_leavetime
-- ----------------------------
DROP TABLE IF EXISTS `t_biz_leavetime`;
CREATE TABLE `t_biz_leavetime`  (
                                    `id` int(11) NOT NULL AUTO_INCREMENT,
                                    `leave_user_id` int(11) NOT NULL,
                                    `leave_user_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
                                    `leave_type` int(11) NOT NULL,
                                    `begin_time` datetime NULL,
                                    `end_time` datetime NULL,
                                    `leavehour` double NOT NULL,
                                    `remark` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                                    `create_user` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
                                    `create_time` timestamp default current_timestamp,
                                    `update_user` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                                    `update_time` timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                                    PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 36 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_biz_purchase
-- ----------------------------
DROP TABLE IF EXISTS `t_biz_purchase`;
CREATE TABLE `t_biz_purchase`  (
                                   `id` int(11) NOT NULL AUTO_INCREMENT,
                                   `purchase_user_id` int(11) NOT NULL,
                                   `purchase_user_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
                                   `purchase_type` int(11) NOT NULL default 1,
                                   `purchase_time` timestamp default current_timestamp,
                                   `plan_procurement_total_money` double NOT NULL default 0,
                                   `remark` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                                   `create_user` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
                                   `create_time` timestamp default current_timestamp,
                                   `update_user` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                                   `update_time` timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                                   PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_biz_ucar_refuel
-- ----------------------------
DROP TABLE IF EXISTS `t_biz_ucar_refuel`;
CREATE TABLE `t_biz_ucar_refuel` (
                                     `id` int(11) NOT NULL AUTO_INCREMENT,
                                     `license_plate_number` varchar(32) DEFAULT NULL COMMENT '车牌号',
                                     `refuel_time` datetime DEFAULT NULL COMMENT '加油日期',
                                     `remark` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                                     `create_user` varchar(50) DEFAULT NULL COMMENT '创建人',
                                     `create_time` datetime DEFAULT NULL COMMENT '创建日期',
                                     `update_user` varchar(50) DEFAULT NULL COMMENT '更新人',
                                     `update_time` datetime DEFAULT NULL COMMENT '更新日期',
                                     PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='加油表';

-- ----------------------------
-- Table structure for t_biz_refund
-- ----------------------------
DROP TABLE IF EXISTS `t_biz_refund`;
CREATE TABLE `t_biz_refund`  (
                                 `id` int(11) NOT NULL AUTO_INCREMENT,
                                 `refund_user_id` int(11) NOT NULL,
                                 `refund_user_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
                                 `refund_type` int(11) NOT NULL default 1,
                                 `refund_date` timestamp NOT NULL,
                                 `refund_money` double NOT NULL default 0,
                                 `remark` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                                 `create_user` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
                                 `create_time` timestamp default current_timestamp,
                                 `update_user` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                                 `update_time` timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                                 PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

create table t_bpmn_conf_lf_formdata
(
    id bigint auto_increment,
    bpmn_conf_id bigint not null,
    formdata longtext null,
    is_del tinyint default 0 not null,
    create_user varchar(255) null,
    create_time timestamp default current_timestamp,
    update_user varchar(255) null,
    update_time timestamp default current_timestamp ON UPDATE CURRENT_TIMESTAMP,
    constraint t_bpmn_conf_lf_formdata_pk
        primary key (id)
)ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

create table if not exists t_bpmn_conf_lf_formdata_field
(
    id bigint auto_increment,
    bpmn_conf_id bigint null,
    formdata_id bigint null,
    field_id varchar(255) null,
    field_name varchar(255) null,
    field_type tinyint null,
    is_condition tinyint default 0 null comment '是否是流程条件,0否,1是',
    is_del tinyint default 0 not null,
    create_user varchar(255) null,
    create_time timestamp default current_timestamp,
    update_user varchar(255) null,
    update_time timestamp default current_timestamp ON UPDATE CURRENT_TIMESTAMP,
    constraint t_bpmn_conf_lf_formdata_field_pk
    primary key (id)
    )ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 comment '低代码配置字段明细表';


create table if not exists t_bpmn_node_lf_formdata_field_control
(
    id bigint auto_increment,
    node_id bigint not null,
    formdata_id bigint not null,
    field_id    varchar(100)  null comment '字段id',
    field_name varchar(255) null comment '字段名',
    field_perm  varchar(10)  null comment '字段权限,是否显示,是否可编辑',
    is_del tinyint default 0 not null,
    create_user varchar(255) null,
    create_time timestamp default current_timestamp,
    update_user varchar(255) null,
    update_time timestamp default current_timestamp ON UPDATE CURRENT_TIMESTAMP,
    constraint t_bpmn_node_lf_formdata_field_control_pk
    primary key (id)
    )ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- 此表为路由表,通过lf.main.table.count控制,默认为2个,索引从0开始,需要自己手动创建
-- ----------------------------
create table t_lf_main
(
    id bigint auto_increment,
    conf_id bigint null,
    form_code varchar(255) null,
    is_del tinyint default 0 not null,
    create_user varchar(255) null,
    create_time timestamp default current_timestamp,
    update_user varchar(255) null,
    update_time timestamp default current_timestamp ON UPDATE CURRENT_TIMESTAMP,
    constraint t_lf_main_pk
        primary key (id)
)ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 comment '低代码表单主表';

-- ----------------------------
-- 此表为路由表,通过lf.field.table.count控制,默认为10个,索引从0开始,需要自己手动创建
-- ----------------------------
create table t_lf_main_field
(
    id bigint auto_increment,
    main_id bigint not null,
    form_code varchar(255) null,
    field_id varchar(255) null,
    field_name varchar(255) null,
    parent_field_id varchar(255) null,
    parent_field_name varchar(255) null,
    field_value varchar(2000) null,
    field_value_number double(14,2) null,
    field_value_dt timestamp null,
    field_value_text longtext null,
    sort int default 0 not null,
    is_del tinyint default 0 not null,
    create_user varchar(255) null,
    create_time timestamp default current_timestamp,
    update_user varchar(255) null,
    update_time timestamp default current_timestamp ON UPDATE CURRENT_TIMESTAMP,
    constraint t_lf_main_field_pk
        primary key (id)
)ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 comment '低代码表单字段值表';

create table t_dict_main
(
    id          bigint auto_increment comment '字典主键'
        primary key,
    dict_name   varchar(100) default ''                null comment '字典名称',
    dict_type   varchar(100) default ''                null comment '字典类型',
    is_del      tinyint      default 0                 not null,
    create_user varchar(255)                           null,
    create_time timestamp    default CURRENT_TIMESTAMP not null,
    update_user varchar(255)                           null,
    update_time timestamp    default CURRENT_TIMESTAMP not null on update CURRENT_TIMESTAMP,
    remark      varchar(500)                           null comment '备注',
    constraint dict_type
        unique (dict_type)
) comment '字典类型表,仅作展示之用,用户可以替换为自己的字段表,能查出需要的内容就行了';

create table t_dict_data
(
    id          bigint auto_increment comment '字典编码'
        primary key,
    dict_sort   int(4)       default 0                 null comment '字典排序',
    dict_label  varchar(100) default ''                null comment '字典标签',
    dict_value  varchar(100) default ''                null comment '字典键值',
    dict_type   varchar(100) default ''                null comment '字典类型',
    css_class   varchar(100)                           null comment '样式属性（其他样式扩展）',
    list_class  varchar(100)                           null comment '表格回显样式',
    is_default  char         default 'N'               null comment '是否默认（Y是 N否）',
    is_del      tinyint      default 0                 not null,
    create_user varchar(255)                           null,
    create_time timestamp    default CURRENT_TIMESTAMP,
    update_user varchar(255)                           null,
    update_time timestamp    default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,
    remark      varchar(500)                           null comment '备注'
) comment '字典表子表,用于存储字典值,一般现有系统都有自己的字典表,可以替换掉,给出sql能查出需要的数据就可以了';

ALTER TABLE bpm_process_node_submit ADD INDEX idx_processInstance_Id(processInstance_Id);

SET FOREIGN_KEY_CHECKS = 1;

-- ----------------------------
-- Table structure for t_bpmn_conf_lf_formdata
-- ----------------------------
DROP TABLE IF EXISTS `t_bpmn_conf_lf_formdata`;
CREATE TABLE `t_bpmn_conf_lf_formdata`  (
                                            `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                            `bpmn_conf_id` bigint(20) NOT NULL,
                                            `formdata` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
                                            `is_del` tinyint(4) NOT NULL DEFAULT 0,
                                            `create_user` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                                            `create_time` timestamp DEFAULT CURRENT_TIMESTAMP,
                                            `update_user` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                                            `update_time` timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                                            PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 32 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;



-- ----------------------------
-- Table structure for t_bpmn_conf_lf_formdata_field
-- ----------------------------
DROP TABLE IF EXISTS `t_bpmn_conf_lf_formdata_field`;
CREATE TABLE `t_bpmn_conf_lf_formdata_field`  (
                                                  `id` bigint(20) NOT NULL AUTO_INCREMENT,
                                                  `bpmn_conf_id` bigint(20) NULL DEFAULT NULL,
                                                  `formdata_id` bigint(20) NULL DEFAULT NULL,
                                                  `field_id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                                                  `field_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                                                  `field_type` tinyint(4) NULL DEFAULT NULL,
                                                  `is_condition` tinyint(4) NULL DEFAULT 0 COMMENT '是否是流程条件,0否,1是',
                                                  `is_del` tinyint(4) NOT NULL DEFAULT 0,
                                                  `create_user` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                                                  `create_time` timestamp DEFAULT CURRENT_TIMESTAMP,
                                                  `update_user` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                                                  `update_time` timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP(0),
                                                  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 28 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '低代码配置字段明细表' ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;


-- ----------------------------
-- Table structure for t_user_role
-- ----------------------------
DROP TABLE IF EXISTS `t_user_role`;
CREATE TABLE `t_user_role`  (
                                `id` int(11) NOT NULL AUTO_INCREMENT,
                                `user_id` int(11) NULL DEFAULT NULL COMMENT ' user id ',
                                `role_id` int(11) NULL DEFAULT NULL COMMENT 'role id',
                                PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 19 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '用户角色关联表' ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;

create table t_bpmn_node_labels
(
    id bigint auto_increment,
    nodeid bigint null,
    label_name varchar(50) null,
    label_value varchar(64) null comment 'a user defined tag',
    remark            varchar(255)        default ''                not null comment '备注',
    is_del            tinyint(1) unsigned default 0                 not null comment '0:正常,1:删除',
    create_user       varchar(32)         default ''                null comment '创建人（邮箱前缀）',
    create_time       timestamp           default CURRENT_TIMESTAMP not null comment '创建时间',
    update_user       varchar(32)         default ''                null comment '更新人（邮箱前缀）',
    update_time       timestamp           default CURRENT_TIMESTAMP not null on update CURRENT_TIMESTAMP comment '更新时间',
    constraint t_bpmn_node_labels_pk
        primary key (id)
)
    comment 'process node labels,to store additional custom information';
create index indx_node_id
    on t_bpmn_node_labels (nodeid);

create table t_bpm_dynamic_condition_choosen
(
    id bigint auto_increment,
    process_number varchar(255) null comment '流程编号',
    node_id varchar(100) null comment '被选中条件节点的id',
    node_from      varchar(100) null,
    constraint t_bpm_dynamic_condition_choosen_pk
        primary key (id)
)
    comment '流程动态条件选择条件记录表';
create index indx_process_number
    on t_bpm_dynamic_condition_choosen (process_number);

alter table t_bpmn_node_conditions_param_conf
    add operator int null after condition_param_jsom;
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
    verify_desc            varchar(2000)           null,
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
    update_user       varchar(64)            null
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
