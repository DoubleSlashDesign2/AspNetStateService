using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetStateService.EntityFrameworkCore
{

    /// <summary>
    /// Data object of a state object.
    /// </summary>
    [Table("StateData")]
    public class StateData
    {

        /// <summary>
        /// ID of the state data object.
        /// </summary>
        [Key]
        [Column("Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual string Id { get; set; }

        /// <summary>
        /// Reference back to the parent.
        /// </summary>
        [ForeignKey(nameof(Id))]
        public virtual State State { get; set; }

        /// <summary>
        /// Persisted data.
        /// </summary>
        [Column("Value")]
        public virtual byte[] Value { get; set; }

    }

}
