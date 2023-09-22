using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCore.Domain;

public abstract class BaseDomainModel
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }

    [ConcurrencyCheck]
    public Guid Version { get; set; }

}

