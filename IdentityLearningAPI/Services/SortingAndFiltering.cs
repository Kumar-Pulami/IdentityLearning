using System.Collections.Specialized;
using System.Dynamic;
using System.Reflection;

namespace IdentityLearningAPI.Services
{
    public static class SortingAndFiltering<T>
    {
        public static List<T> Sort(List<T> sortableList, List<SortMethodType> sortingMethods) {
            if (sortableList == null)
            {
                throw new NullReferenceException(nameof(sortableList));
            }

            if (sortingMethods == null)
            {
                throw new NullReferenceException(nameof(sortingMethods));
            }

            List<T> currentSortedList = sortableList;
            Type type = typeof(T);
            PropertyInfo propertyInfo;

            foreach (SortMethodType sortMethodType in sortingMethods)
            {
                if (sortMethodType.ByAscending)
                {
                    currentSortedList = sortableList.OrderBy(x => x.GetType()
                                       .GetProperty(sortMethodType.PropertyName)
                                       .GetValue(x, null)).ToList();
                }
                else
                {
                    currentSortedList = sortableList.OrderByDescending(x => x.GetType()
                                     .GetProperty(sortMethodType.PropertyName)
                                     .GetValue(x, null)).ToList();
                }
            }

            return currentSortedList;
        }

        private static List<dynamic> Sort1(List<dynamic> input, string property)
{
            return input.OrderBy(p => p.GetType()
                                       .GetProperty(property)
                                       .GetValue(p, null)).ToList();
        }
    }


    public class SortMethodType {

        public string PropertyName { get; set; }

        public bool ByAscending { get; set; }

    }
}
