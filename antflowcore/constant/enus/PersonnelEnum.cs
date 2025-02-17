namespace antflowcore.constant.enus;

public enum PersonnelEnum
    {
        NODE_LOOP_PERSONNEL,
        NODE_LEVEL_PERSONNEL,
        ROLE_PERSONNEL,
        USERAPPOINTED_PERSONNEL,
        CUSTOMIZABLE_PERSONNEL,
        HRBP_PERSONNEL,
        OUT_SIDE_ACCESS_PERSONNEL,
        START_USER_PERSONNEL,
        DIRECT_LEADER_PERSONNEL,
        BUSINESS_TABLE_PERSONNEL
    }

    public static class PersonnelEnumExtensions
    {
        private static readonly Dictionary<PersonnelEnum, (NodePropertyEnum NodeProperty, string Description)> PersonnelMappings = 
            new Dictionary<PersonnelEnum, (NodePropertyEnum, string)>
        {
            { PersonnelEnum.NODE_LOOP_PERSONNEL, (NodePropertyEnum.NODE_PROPERTY_LOOP, "层层审批") },
            { PersonnelEnum.NODE_LEVEL_PERSONNEL, (NodePropertyEnum.NODE_PROPERTY_LEVEL, "指定层级审批") },
            { PersonnelEnum.ROLE_PERSONNEL, (NodePropertyEnum.NODE_PROPERTY_ROLE, "指定角色") },
            { PersonnelEnum.USERAPPOINTED_PERSONNEL, (NodePropertyEnum.NODE_PROPERTY_PERSONNEL, "指定人员") },
            { PersonnelEnum.CUSTOMIZABLE_PERSONNEL, (NodePropertyEnum.NODE_PROPERTY_CUSTOMIZE, "发起人自选") },
            { PersonnelEnum.HRBP_PERSONNEL, (NodePropertyEnum.NODE_PROPERTY_HRBP, "HRBP") },
            { PersonnelEnum.OUT_SIDE_ACCESS_PERSONNEL, (NodePropertyEnum.NODE_PROPERTY_OUT_SIDE_ACCESS, "外部传入人员") },
            { PersonnelEnum.START_USER_PERSONNEL, (NodePropertyEnum.NODE_PROPERTY_START_USER, "发起人自己") },
            { PersonnelEnum.DIRECT_LEADER_PERSONNEL, (NodePropertyEnum.NODE_PROPERTY_DIRECT_LEADER, "直属领导") },
            { PersonnelEnum.BUSINESS_TABLE_PERSONNEL, (NodePropertyEnum.NODE_PROPERTY_BUSINESSTABLE, "关联业务表") }
        };

        public static NodePropertyEnum GetNodeProperty(this PersonnelEnum personnelEnum)
        {
            return PersonnelMappings[personnelEnum].NodeProperty;
        }

        public static string GetDescription(this PersonnelEnum personnelEnum)
        {
            return PersonnelMappings[personnelEnum].Description;
        }

        public static PersonnelEnum? FromNodePropertyEnum(NodePropertyEnum nodePropertyEnum)
        {
            return PersonnelMappings.FirstOrDefault(x => x.Value.NodeProperty == nodePropertyEnum).Key;
        }
    }