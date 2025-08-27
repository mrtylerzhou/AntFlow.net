using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor;

public interface IFormOperationAdaptor<in T> where T : BusinessDataVo
{
    // ����Ԥ������
    BpmnStartConditionsVo PreviewSetCondition(T vo);

    // ��������
    BpmnStartConditionsVo LaunchParameters(T vo);

    // ��ʼ������
    void OnInitData(T vo);

    // ��ѯ����
    void OnQueryData(T vo);

    // �ύ����
    void OnSubmitData(T vo);

    // ͬ������ʱ�Ļص�
    void OnConsentData(T vo);

    // ���ص��޸�ʱ�Ļص�
    void OnBackToModifyData(T vo);

    // ȡ������ʱ�Ļص�
    void OnCancellationData(T vo);

    // ���̽���ʱ�Ļص�
    void OnFinishData(BusinessDataVo vo);
}