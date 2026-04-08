namespace AntFlowCore.Base.factory;

public interface TagParser<out TService, in TParam> {
    TService ParseTag(TParam data);
}