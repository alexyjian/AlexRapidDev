using System;

namespace ALEXFW.Entity.CommonDictionary
{
    /// <summary>
    ///     适用于实体只用GUID 关联的字段
    ///     字段的 特性需要有 ：[CustomDataType("EntityID")]
    /// </summary>
    public class EntityMultiSelectAttribute : Attribute
    {
        public Type[] Types { get; set; }
    }
}