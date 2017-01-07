using LootMart.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LootMart.Ark
{
    public static class ArkCreation
    {
        public static Guid SpawnEntityWithOverrides(string xmlFilePath, ref ECSContainer ecsContainer, BaseEntity additions = null)
        {
            Guid id = ArkCreation.CreateEntityFromFile(xmlFilePath, ref ecsContainer);
            if (additions != null)
            {
                ecsContainer.AppendEntity(additions, id);
            }
            return id;
        }

        public static Guid CreateEntityFromFile(string xmlFilePath, ref ECSContainer ecsContainer)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BaseEntity));
            using (FileStream fileStream = new FileStream(xmlFilePath, FileMode.Open))
            {
                BaseEntity entity = (BaseEntity)serializer.Deserialize(fileStream);
                return ecsContainer.AddEntity(entity);
            }
        }
    }
}
