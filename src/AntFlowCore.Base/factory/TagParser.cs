namespace AntFlowCore.Core.factory;

public interface TagParser<out TService, in TParam> {
    TService ParseTag(TParam data);
}