
using rehome.Models.DB;
using rehome.Public;
using X.PagedList;

namespace rehome.Models
{
    public class ClientIndexModel
    {
        public ClientSearchConditions ClientSearchConditions { get; set; } = new ClientSearchConditions();
        public IList<顧客>? Clients { get; set; }

        public int? Clientcount {  get; set; } 

    }
}
