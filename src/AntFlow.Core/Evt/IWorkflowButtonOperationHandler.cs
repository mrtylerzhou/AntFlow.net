using AntFlow.Core.Vo;

namespace AntFlow.Core.Event;

/// <summary>
///     ???????????????????
/// </summary>
public interface IWorkflowButtonOperationHandler
{
    /// <summary>
    ///     ??????
    /// </summary>
    void onSubmit(BusinessDataVo businessData);

    /// <summary>
    ///     ??????
    /// </summary>
    void onResubmit(BusinessDataVo businessData);

    /// <summary>
    ///     ???
    /// </summary>
    void onAgree(BusinessDataVo businessData);

    /// <summary>
    ///     ?????
    /// </summary>
    void onDisAgree(BusinessDataVo businessData);

    /// <summary>
    ///     ??????????
    /// </summary>
    void onViewBusinessProcess(BusinessDataVo businessData);

    /// <summary>
    ///     ????
    /// </summary>
    void onAbandon(BusinessDataVo businessData);

    /// <summary>
    ///     ?§Ñ?
    /// </summary>
    void onUndertake(BusinessDataVo businessData);

    /// <summary>
    ///     ?????????
    /// </summary>
    void onChangeAssignee(BusinessDataVo businessData);

    /// <summary>
    ///     ???
    /// </summary>
    void onStop(BusinessDataVo businessData);

    /// <summary>
    ///     ???
    /// </summary>
    void onForward(BusinessDataVo businessData);

    /// <summary>
    ///     ??????
    /// </summary>
    void onBackToModify(BusinessDataVo businessData);

    /// <summary>
    ///     ???????
    /// </summary>
    void onProcessDrawBack(BusinessDataVo businessData);

    /// <summary>
    ///     ????
    /// </summary>
    void onJp(BusinessDataVo businessData);

    /// <summary>
    ///     ???
    /// </summary>
    void onZb(BusinessDataVo businessData);

    /// <summary>
    ///     ?????????
    /// </summary>
    void onChooseAssignee(BusinessDataVo businessData);

    /// <summary>
    ///     ?????????
    /// </summary>
    void onBackToAnyNode(BusinessDataVo businessData);

    /// <summary>
    ///     ???
    /// </summary>
    void onRemoveAssignee(BusinessDataVo businessData);

    /// <summary>
    ///     ???
    /// </summary>
    void onAddAssignee(BusinessDataVo businessData);

    /// <summary>
    ///     ???¦Ä???????????
    /// </summary>
    void onChangeFutureAssignee(BusinessDataVo businessData);

    /// <summary>
    ///     ¦Ä???????
    /// </summary>
    void onRemoveFutureAssignee(BusinessDataVo businessData);

    /// <summary>
    ///     ¦Ä???????
    /// </summary>
    void onAddFutureAssignee(BusinessDataVo businessData);

    /// <summary>
    ///     ???????
    /// </summary>
    void onFinishData(BusinessDataVo vo);
}