namespace antflowcore.entityj;

public class DetailedUser
{
    public String Id{get;set;}
    public String UserName{get;set;}
    /**
     * direct leader id
     */
    public long? LeaderId{get;set;}
    /**
     * emaill,for notification
     */
    public String Email{get;set;}
    /**
     * moible
     */
    public String Mobile{get;set;}
    public int? IsDel{get;set;}
    /**
     * hrbp id
     */
    public int? HrbpId{get;set;}
    /**
     * avatar
     */
    public String HeadImg{get;set;}
    /**
     * is show mobile,you should respect user's privacy,if he or she do not want to show his or her mobile,you should set it false
     */
    public Boolean MobileIsShow{get;set;}
    public String Path{get;set;}
    public int? DepartmentId{get;set;}
}