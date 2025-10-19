using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum EstadoEnum
{
    _,
    FALSO,
    VERDADERO,
    RESUELTO
}

[Serializable]
public enum ObjetoClaveEnum
{
    _,
    FALSO,
    VERDADERO,
    USADO
}

[Serializable]
public class Moneda
{
    public string nombre;
    public int valor;
}

[Serializable]
public class Estado
{
    public string nombre;
    public EstadoEnum valor = EstadoEnum._;
}

[Serializable]
public class ObjetoClave
{
    public string nombre;
    public ObjetoClaveEnum valor = ObjetoClaveEnum._;
}

[Serializable]
public class EventoDiccionario
{
    public string nombre;
    public bool valor;
}

[Serializable]
public class Evento
{
    public bool isLoopPersistent;
    public bool startValue;
    public List<EventoDiccionario> diccionario = new List<EventoDiccionario>();
}

[Serializable]
public class Objetos
{
    public List<Moneda> monedas = new List<Moneda>();
    public List<Estado> estados = new List<Estado>();
    public List<ObjetoClave> objetosClave = new List<ObjetoClave>();
}

[CreateAssetMenu(fileName = "NuevoInventario", menuName = "Inventario/InventarioObject")]
public class InventarioObject : ScriptableObject
{
    public Objetos objetos = new Objetos();
    public List<Evento> eventos = new List<Evento>();

    // ----------------Métodos de utilidad para acceder a los datos----------------
    
    /// <summary>
    /// Permite tomar el valor de una moneda dado el nombre de la misma.
    /// </summary>
    /// <param name="nombre">Nombre de la moneda a recibir</param>
    /// <returns>Valor de la moneda o "-1" si no encuentra</returns>
    public int ObtenerValorMoneda(string nombre)
    {
        var moneda = objetos.monedas.Find(m => m.nombre == nombre);
        return moneda?.valor ?? -1;
    }

    /// <summary>
    /// Permite modificar el valor de una moneda, siempre mayor que 0.
    /// </summary>
    /// <param name="nombre">Nombre de la moneda a cambiar</param>
    /// <param name="cantidad">Valor que se añadirá a la cantidad actual</param>
    /// <returns>Valor final de la moneda, "-1" si no la encuentra</returns>
    public int ModificarMoneda(string nombre, int cantidad)
    {
        var moneda = objetos.monedas.Find(m => m.nombre == nombre);
        if (moneda != null)
        {
            moneda.valor += cantidad;
            if(moneda.valor < 0) moneda.valor = 0;
        }

        return moneda?.valor ?? -1;
    }

    /// <summary>
    /// Permite tomar el valor de un estado dado el nombre del mismo.
    /// </summary>
    /// <param name="nombre">Nombre de la moneda a recibir</param>
    /// <returns>Valor de la moneda o "_" si no encuentra</returns>
    public EstadoEnum ObtenerEstado(string nombre)
    {
        var estado = objetos.estados.Find(e => e.nombre == nombre);
        return estado?.valor ?? EstadoEnum._;
    }

    /// <summary>
    /// Permite modificar el valor de un estado.
    /// </summary>
    /// <param name="nombre">Nombre del estado a cambiar</param>
    /// <param name="nuevoValor">Valor que se establecerá como actual</param>
    /// <returns>Valor final del estado, "_" si no lo encuentra</returns>
    public EstadoEnum CambiarEstado(string nombre, EstadoEnum nuevoValor)
    {
        var estado = objetos.estados.Find(e => e.nombre == nombre);
        if (estado != null)
        {
            estado.valor = nuevoValor;
        }
        
        return estado?.valor ?? EstadoEnum._;
    }

    /// <summary>
    /// Permite tomar el valor de un objeto clave dado el nombre del mismo.
    /// </summary>
    /// <param name="nombre">Nombre del objeto clave a recibir</param>
    /// <returns>Valor del objeto clave o "_" si no encuentra</returns>
    public ObjetoClaveEnum ObtenerObjetoClave(string nombre)
    {
        var objeto = objetos.objetosClave.Find(o => o.nombre == nombre);
        return objeto?.valor ?? ObjetoClaveEnum._;
    }

    /// <summary>
    /// Permite modificar el valor de un objeto clave.
    /// </summary>
    /// <param name="nombre">Nombre del objeto clave a cambiar</param>
    /// <param name="nuevoValor">Valor que se establecerá como actual</param>
    /// <returns>Valor final del objeto clave, "_" si no lo encuentra</returns>
    public ObjetoClaveEnum CambiarObjetoClave(string nombre, ObjetoClaveEnum nuevoValor)
    {
        var objeto = objetos.objetosClave.Find(o => o.nombre == nombre);
        if (objeto != null)
        {
            objeto.valor = nuevoValor;
        }
        return  objeto?.valor ?? ObjetoClaveEnum._;
    }

    /// <summary>
    /// Obtiene la referencia del evento dado un nombre
    /// </summary>
    /// <param name="nombreEvento">El nombre del evento</param>
    /// <returns>Referencia de tipo Evento</returns>
    public Evento ObtenerEventoPorNombre(string nombreEvento)
    {
        return eventos.Find(e => e.diccionario.Count > 0 && e.diccionario[0].nombre == nombreEvento);
    }

    /// <summary>
    /// Devuelve el valor de la persistencia en el bucle del evento
    /// </summary>
    /// <param name="nombreEvento">El nombre del evento</param>
    /// <returns>bool?: Si el evento persiste en el bucle o null si no encuentra el evento</returns>
    public bool? ObtenerIsLoopPersistent(string nombreEvento)
    {
        var evento = ObtenerEventoPorNombre(nombreEvento);
        return evento?.isLoopPersistent;
    }

    /// <summary>
    /// Devuelve el valor inicial del evento
    /// </summary>
    /// <param name="nombreEvento">El nombre del evento</param>
    /// <returns>bool?: Valor inicial del evento o null si no encuentra el evento</returns>
    public bool? ObtenerStartValue(string nombreEvento)
    {
        var evento = ObtenerEventoPorNombre(nombreEvento);
        return evento?.startValue;
    }

    /// <summary>
    /// Devuelve el valor actual del evento
    /// </summary>
    /// <param name="nombreEvento">El nombre del evento</param>
    /// <returns>bool?: Valor actual del evento o null si no encuentra el evento o no tiene valor</returns>
    public bool? ObtenerValorEvento(string nombreEvento)
    {
        var evento = ObtenerEventoPorNombre(nombreEvento);
        if(evento == null)  return null;
        if (evento.diccionario.Count > 0)
        {
            return evento.diccionario[0].valor;
        }
        return null;
    }

    /// <summary>
    /// Cambia el valor de la persistencia en el bucle del evento
    /// </summary>
    /// <param name="nombreEvento">El nombre del evento</param>
    /// <param name="nuevoValor">El nuevo valor de persistencia en el bucle</param>
    /// <returns>bool?: El nuevo valor asignado o null si no encuentra el evento</returns>
    public bool? CambiarIsLoopPersistent(string nombreEvento, bool nuevoValor)
    {
        var evento = ObtenerEventoPorNombre(nombreEvento);
        if (evento != null)
        {
            evento.isLoopPersistent = nuevoValor;
        }
        return evento?.isLoopPersistent;
    }

    /// <summary>
    /// Cambia el valor actual del evento
    /// </summary>
    /// <param name="nombreEvento">El nombre del evento</param>
    /// <param name="nuevoValor">El nuevo valor del evento</param>
    /// <returns>bool?: Valor actual del evento o null si no encuentra el evento o no tiene valor</returns>
    public bool? CambiarValorEvento(string nombreEvento, bool nuevoValor)
    {
        var evento = ObtenerEventoPorNombre(nombreEvento);
        if(evento == null) return null;
        if (evento.diccionario.Count > 0)
        {
            evento.diccionario[0].valor = nuevoValor;
        }
        return evento?.isLoopPersistent;
    }
}