

namespace glTFLoader.Schema
{
    public class AccessorSparse
    {

        /// <summary>
        /// Backing field for Count.
        /// </summary>
        private int m_count;

        /// <summary>
        /// Backing field for Indices.
        /// </summary>
        private AccessorSparseIndices m_indices;

        /// <summary>
        /// Backing field for Values.
        /// </summary>
        private AccessorSparseValues m_values;

        /// <summary>
        /// Backing field for Extensions.
        /// </summary>
        private System.Collections.Generic.Dictionary<string, object> m_extensions;

        /// <summary>
        /// Backing field for Extras.
        /// </summary>
        private Extras m_extras;

        /// <summary>
        /// Number of entries stored in the sparse array.
        /// </summary>
        [Newtonsoft.Json.JsonRequiredAttribute()]
        [Newtonsoft.Json.JsonPropertyAttribute("count")]
        public int Count
        {
            get => m_count;
            set
            {
                if (value < 1)
                {
                    throw new System.ArgumentOutOfRangeException("Count", value, "Expected value to be greater than or equal to 1");
                }
                m_count = value;
            }
        }

        /// <summary>
        /// Index array of size `count` that points to those accessor attributes that deviate from their initialization value. Indices must strictly increase.
        /// </summary>
        [Newtonsoft.Json.JsonRequiredAttribute()]
        [Newtonsoft.Json.JsonPropertyAttribute("indices")]
        public AccessorSparseIndices Indices
        {
            get => this.m_indices;
            set => this.m_indices = value;
        }

        /// <summary>
        /// Array of size `count` times number of components, storing the displaced accessor attributes pointed by `indices`. Substituted values must have the same `componentType` and number of components as the base accessor.
        /// </summary>
        [Newtonsoft.Json.JsonRequiredAttribute()]
        [Newtonsoft.Json.JsonPropertyAttribute("values")]
        public AccessorSparseValues Values
        {
            get => this.m_values;
            set => this.m_values = value;
        }

        /// <summary>
        /// Dictionary object with extension-specific objects.
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("extensions")]
        public System.Collections.Generic.Dictionary<string, object> Extensions
        {
            get => this.m_extensions;
            set => this.m_extensions = value;
        }

        /// <summary>
        /// Application-specific data.
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("extras")]
        public Extras Extras
        {
            get => this.m_extras;
            set => this.m_extras = value;
        }

        public bool ShouldSerializeIndices()
        {
            return m_indices is not null;
        }

        public bool ShouldSerializeValues()
        {
            return ((m_values == null)
                        == false);
        }

        public bool ShouldSerializeExtensions()
        {
            return ((m_extensions == null)
                        == false);
        }

        public bool ShouldSerializeExtras()
        {
            return ((m_extras == null)
                        == false);
        }
    }
}
