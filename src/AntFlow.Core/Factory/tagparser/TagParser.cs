namespace AntFlow.Core.Factory;

public interface TagParser<out TService, in TParam>
{
    TService ParseTag(TParam data);
}