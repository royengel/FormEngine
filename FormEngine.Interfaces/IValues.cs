using System.Dynamic;

namespace FormEngine.Interfaces
{
    public abstract class IValues : DynamicObject
    {
        public abstract string Get(string valueName);

        public override bool TryGetMember(GetMemberBinder binder,
                                  out object result)
        {
            result = Get(binder.Name);
            return result == null ? false : true;
        }
    }
}
