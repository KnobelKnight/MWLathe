using System.Diagnostics.CodeAnalysis;

namespace MWLathe.Helpers
{
    public abstract class AIPackage
    {
        public bool Updated { get; set; } = false;

        public abstract uint GetByteSize();
        public abstract void UpdateID(string oldID, string newID);
        public abstract void Write(FileStream ts);

        [return: NotNullIfNotNull(nameof(field))]
        public string? ReplaceID(string? field, string oldID, string newID)
        {
            if (field is not null && field.Equals(oldID, StringComparison.OrdinalIgnoreCase))
            {
                field = newID;
                Updated = true;
            }
            return field;
        }
    }
}
