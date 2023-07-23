using System.Diagnostics.CodeAnalysis;

namespace IssueManager
{    internal class IssueComparer : IEqualityComparer<Issue>
    {
        public bool Equals(Issue? x, Issue? y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode([DisallowNull] Issue obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
