namespace MusicAdmin.WebAPI
{
    [AttributeUsage(AttributeTargets.Class
   | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class UnitOfWorkAttribute : Attribute
    {
        public Type[] DbConextTypes { get; init; }

        public UnitOfWorkAttribute(params Type[] dbConextTypes)
        {
            DbConextTypes = dbConextTypes;
        }
    }
}
