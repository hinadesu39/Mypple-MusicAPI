namespace FileService.WebAPI
{
    public class UnitOfWorkAttribute : Attribute
    {
        public Type[] DbConextTypes { get; init; }

        public UnitOfWorkAttribute(params Type[] dbConextTypes)
        {
            DbConextTypes = dbConextTypes;
        }
    }
}
