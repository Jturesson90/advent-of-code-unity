using UnityEditor;
using UnityEngine;

namespace Editor
{
    [FilePath("AdventOfCodePagination/AdventOfCodePaginationState.foo",
        FilePathAttribute.Location.PreferencesFolder)]
    public class AdventOfCodePagination : ScriptableSingleton<AdventOfCodePagination>
    {
        [SerializeField] public int hej = 2;
    }
}