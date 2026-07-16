namespace badmintion.Contansts
{
    public class ListActionDefault
    {
        public static List<string> listAction = new List<string>()
    {
        "manage", "create", "update", "delete"
    };



        public static List<string> listActionAPI = new List<string>()
    {
        "create", "update", "get-all-core", "get-all-selected", "get-by-id-core", "get-by-ndt-id","delete","deleted","get-paging-params-core"
    };

        public static string KeyId = "unique_name";

        public static string UnitRoleIdString = "role";
        public static int UnitRoleId = 1;
    }
}
