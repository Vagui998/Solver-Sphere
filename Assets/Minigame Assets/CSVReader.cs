using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VariableEjemplo
{
    public string tipo;
    public string ejemplo;
}

public class CSVReader : MonoBehaviour
{
    public List<VariableEjemplo> LeerCSV(string path)
    {
        List<VariableEjemplo> listaVariables = new List<VariableEjemplo>();

        if (System.IO.File.Exists(path))
        {
            string[] lineas = System.IO.File.ReadAllLines(path);
            foreach (string linea in lineas)
            {
                string[] datos = linea.Split(',');
                VariableEjemplo variable = new VariableEjemplo();
                variable.tipo = datos[1];
                variable.ejemplo = datos[0];
                listaVariables.Add(variable);
            }
            return listaVariables;
        }
        else
        {
            Debug.LogError("No se encontró el archivo.");
            return null;
        }
    }
}
