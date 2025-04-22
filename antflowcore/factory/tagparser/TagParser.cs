namespace antflowcore.factory.tagparser;

public interface TagParser<out TService, in TParam>
{
    TService ParseTag(TParam data);
}