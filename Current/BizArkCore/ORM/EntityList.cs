using System.Collections;
using System.Collections.Generic;

namespace BizArk.Core.ORM
{

    /// <summary>
    /// A lazily loaded list of entities.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityList<T> : IList<T> where T : Entity
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of EntityList.
        /// </summary>
        public EntityList()
        {
        }

        /// <summary>
        /// Creates an instance of EntityList.
        /// </summary>
        /// <param name="entities"></param>
        public EntityList(IEnumerable<T> entities)
        {
            AddRange(entities);
        }

        #endregion

        #region Fields and Properties

        private List<T> mEntities = new List<T>();

        /// <summary>
        /// Gets or sets the entity at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get { return mEntities[index]; }
            set { mEntities[index] = value; }
        }

        /// <summary>
        /// Gets the number of entities in the list.
        /// </summary>
        public int Count
        {
            get { return mEntities.Count; }
        }

        /// <summary>
        /// Gets a value that determines if this list is readonly.
        /// </summary>
        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the index for the given entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int IndexOf(T entity)
        {
            return mEntities.IndexOf(entity);
        }

        /// <summary>
        /// Adds the entity to the list at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="entity"></param>
        public void Insert(int index, T entity)
        {
            mEntities.Insert(index, entity);
        }

        /// <summary>
        /// Removes the entity at the given index. If the FK to the parent in the entity can be null, the value will be nulled, otherwise the record will be deleted.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            mEntities.RemoveAt(index);
        }

        /// <summary>
        /// Adds the entity to the list.
        /// </summary>
        /// <param name="entity"></param>
        public void Add(T entity)
        {
            // Only add an entity to the list once.
            if (!mEntities.Contains(entity))
                mEntities.Add(entity);
        }

        /// <summary>
        /// Adds the entities to the list.
        /// </summary>
        /// <param name="entities"></param>
        public void AddRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
                Add(entity);
        }

        /// <summary>
        /// Removes all the entities from the list.  If the FK to the parent in the entity can be null, the value will be nulled, otherwise the record will be deleted.
        /// </summary>
        public void Clear()
        {
            mEntities.Clear();
        }

        /// <summary>
        /// Determines whether the list contains the specified entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Contains(T entity)
        {
            return mEntities.Contains(entity);
        }

        /// <summary>
        /// Copies the entity to the given array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            mEntities.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the entity from the list. If the FK to the parent in the entity can be null, the value will be nulled, otherwise the record will be deleted.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Remove(T entity)
        {
            return mEntities.Remove(entity);
        }

        /// <summary>
        /// Gets the enumerator for the list.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return mEntities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mEntities.GetEnumerator();
        }

        #endregion

    }

}
