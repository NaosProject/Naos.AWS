// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmiSearchStrategy.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    /// <summary>
    /// AMI search behavior model object.
    /// </summary>
    public class AmiSearchStrategy
    {
        /// <summary>
        /// Gets or sets the pattern used to search available AMI's.
        /// </summary>
        public string SearchPattern { get; set; }

        /// <summary>
        /// Gets or sets the alias of the owner of the AMI (this is to make sure you're only using AMI's from a trusted source).
        /// </summary>
        public string OwnerAlias { get; set; }

        /// <summary>
        /// Gets or sets the behavior when multiple AMI's are found matching the provided pattern.
        /// </summary>
        public MultipleAmiFoundBehavior MultipleFoundBehavior { get; set; }

        /// <summary>
        /// Gets a deep clone of the object.
        /// </summary>
        /// <returns>Deeply cloned version of the object.</returns>
        public AmiSearchStrategy DeepClone()
        {
            var ret = new AmiSearchStrategy()
                          {
                              MultipleFoundBehavior = this.MultipleFoundBehavior,
                              OwnerAlias = this.OwnerAlias,
                              SearchPattern = this.SearchPattern,
                          };

            return ret;
        }
    }
}