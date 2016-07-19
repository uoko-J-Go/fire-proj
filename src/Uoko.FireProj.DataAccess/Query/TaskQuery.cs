namespace Uoko.FireProj.DataAccess.Query
{
    public class TaskQuery : BaseQuery
    {
        public enum QueryType
        {
            ShowAll = 0,
            QaFocus = 2,
            CreatorFocus = 1,
        }

        public QueryType ShowType { get; set; }

        public int LoginUserId { get; set; }
    }


    public class TaskNeedOnlineQuery : BaseQuery
    {
        public int ProjectId { get; set; }

    }
}
