using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Jogo.Componentes
{
    /// <summary>
    /// Classe base para uma lista de Itens serializável
    /// </summary>
    /// <typeparam name="T">O tipo de Item a ser listado</typeparam>
    [Serializable]
    public class ListaItens<T> : List<T>, IXmlSerializable
    {
        #region IXmlSerializable Members
        public XmlSchema GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
        {
            XmlSerializer sr = new XmlSerializer(typeof(T));
            int depth = reader.Depth;

            if (!reader.IsEmptyElement && reader.Read())
            {
                while (reader.Depth > depth)
                {
                    try
                    {
                        if (reader.Depth == depth + 1 && reader.NodeType != XmlNodeType.EndElement)
                        {
                            this.Add((T)sr.Deserialize(reader));
                        }
                        else
                        {
                            reader.Read();
                        }
                    }
                    catch (Exception)
                    {
                        // ignora erros
                    }
                }
            }
            reader.Read();
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer sr = new XmlSerializer(typeof(T));
            foreach (T item in this) sr.Serialize(writer, item);
        }
        #endregion
    }
}
