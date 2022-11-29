using UnityEditor;
using UnityEngine;

namespace Editor
{
    [FilePath("AdventOfCodePagination/AdventOfCodePaginationState.foo",
        FilePathAttribute.Location.PreferencesFolder)]
    public class AdventOfCodePagination : ScriptableSingleton<AdventOfCodePagination>
    {
        public enum PaginationStateEnum
        {
            ShowDescription,
            ShowInput,
            Non
        }

        public int day;

        public string description;
        public string input;
        public PaginationStateEnum paginationState;
    }
}