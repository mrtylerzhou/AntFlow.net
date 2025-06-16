using antflowcore.entity;

namespace antflowcore.service.biz;

public class TaskDataEqualityComparer: EqualityComparer<BpmAfTask>
{
       public override bool Equals(BpmAfTask x, BpmAfTask y)
       {
           if (x == null || y == null) return false;
           return x.TaskDefKey == y.TaskDefKey;
       }
   
       public override int GetHashCode(BpmAfTask obj)
       {
           return obj?.TaskDefKey?.GetHashCode() ?? 0;
       }
       
}