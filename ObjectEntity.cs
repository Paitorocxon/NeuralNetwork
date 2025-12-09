using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPLAI_2
{
    public class ObjectEntity
    {
        public string name { get; set; } = "Nameless Entity";
        public string description { get; set; } = "Unknown Object";
        public string shortDescription { get; set; } = "Unknown Object";
        public bool plural { get; set; } = false;
        public int gender { get; set; } = 0;
        
        public List<string> properties = new List<string>();            // properties like "is big, is a man"
        public List<ObjectEntity> inputNode = new List<ObjectEntity>();  // relations where it comes from
        public List<ObjectEntity> outputNode = new List<ObjectEntity>(); // relations which base on this entity
        
        public ObjectEntity(string name)
        {
            this.name = name;
            this.gender = 0;
        }

        /// <summary>
        ///  Add Parent Relation.
        ///  Required to add a Relation to a origin of an entity.
        /// </summary>
        /// <param name="entity"></param>
        public void AddInputNode(ObjectEntity entity)
        {
            if (!inputNode.Contains(entity))
            {
                inputNode.Add(entity);
            }
        }


        /// <summary>
        ///  Add Child Relation.
        ///  Required to add a Relation to a childs-origin of an entity.
        /// </summary>
        /// <param name="entity"></param>
        public void AddOutputNode(ObjectEntity entity)
        {
            if (!outputNode.Contains(entity))
            {
                outputNode.Add(entity);
            }
        }


        /// <summary>
        /// Checks if the Entity is meant.
        /// </summary>
        /// <param name="requestedName"></param>
        /// <returns></returns>
        public bool AreYou(string requestedName)
        {
            string[] singleNameArray = new string[] { name };
            return name == WordProcessing.mostPossibleWord(singleNameArray, requestedName);

        }

    }
}
