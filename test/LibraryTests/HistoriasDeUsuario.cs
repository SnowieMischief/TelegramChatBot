namespace LibraryTests;
using Library;

/// <summary> Tests de los escenarios (casos de usuario) dados </summary>
public class HistoriasDeUsuario
{
    [SetUp]
    public void Setup()
    {
    }
    
    [TearDown]
    public void Wipe()
    {
        Administrador root = new Administrador("root", "toor", "0130123", "abc@acb.cba");
        Categoria.Wipe(root);
        CategoriasCatalog.Wipe(root);
        ContratoHandler.Wipe(root);
        OfertaDeServicio.Wipe(root);
        OfertasHandler.Wipe(root);
        RegistryHandler.Wipe(root);
        Solicitud.Wipe(root);
        SolicitudCatalog.Wipe(root);
        UsuariosCatalog.Wipe(root);
    }
    
    [Test]
    public void AdministradorCrearCategoria()
    // "Cómo administrador, quiero poder indicar categorías sobre las cuales se realizarán las ofertas de servicios
    // para que de esa forma, los trabajadores puedan clasificarlos.
    {
        // Arrange
        RegistryHandler registryHandler = RegistryHandler.GetInstance();
        Administrador admin = registryHandler.RegistrarAdministrador("root", "toor", "1234", "abc@abc.com");
        OfertasHandler ofertasHandler = OfertasHandler.GetInstance();
        bool expected = true;

        // Act
        Categoria cat = ofertasHandler.CrearCategoria(admin, "Tareas del hogar");
        bool result = ofertasHandler.GetCategorias().Contains(cat);

        // Assert
        Assert.That(expected.Equals(result));
    }

    [Test]
    public void AdministradorDarDeBajaOferta()
    // Como administrador, quiero poder dar de baja ofertas de servicios, avisando al oferente para que de esa forma,
    // pueda evitar ofertas inadecuadas.
    {
        // Arrange
        RegistryHandler registryHandler = RegistryHandler.GetInstance();
        Administrador admin = registryHandler.RegistrarAdministrador("root", "toor", "1234", "abc@abc.com");
        OfertasHandler ofertasHandler = OfertasHandler.GetInstance();
        Categoria categoria = ofertasHandler.CrearCategoria(admin, "Tareas");
        Trabajador elpepe = registryHandler.RegistrarTrabajador("Pepe", "Pepe", "Elpepe", "elpepe", "2020,10,1", "12345678", "1234",
            "elpepe@elpepe.elpepe", new Tuple<double, double>(31,9393));
        OfertaDeServicio oferta = ofertasHandler.Ofertar(categoria.GetId(), elpepe, "un capo", "limpiador", 10);
        int id = oferta.GetId();
        bool expected = false;
        
        // Act
        ofertasHandler.DarDeBajaOferta(admin,id);
        bool result = ofertasHandler.GetOfertaById(id).IsActive();

        // Assert
        Assert.That(expected.Equals(result));
   
    }

    [Test]
    public void TrabajadorRegistrarse()
    // Como trabajador, quiero registrarme en la plataforma, indicando mis datos personales e información de contacto
    // para que de esa forma, pueda proveer información de contacto a quienes quieran contratar mis servicios.
    { 
        // Arrange
        RegistryHandler registryHandler = RegistryHandler.GetInstance();
        bool Expected = true;
        
        // Act
        Usuario miUsuario = registryHandler.RegistrarTrabajador("Manolo","Manolete", "manoler","1234",
            "2001 3 14","1234567","099555555",
            "manoloreal@gmail.com",new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        bool Result = (miUsuario is Trabajador);
        
        // Assert
        Assert.That(Result.Equals(Expected));
 
    }

    [Test]
    public void TrabajadorCrearOferta()
    // Como trabajador, quiero poder hacer ofertas de servicios; mi oferta indicará en qué categoría quiero publicar,
    // tendrá una descripción del servicio ofertado, y un precio para que de esa forma, mis ofertas sean ofrecidas a
    // quienes quieren contratar servicios.
    { 
        // Arrange
        OfertasHandler ofertasHandler = OfertasHandler.GetInstance();
        RegistryHandler registryHandler = RegistryHandler.GetInstance();
        Usuario miUsuario = registryHandler.RegistrarTrabajador("Manolo","Manolete", "manoler","1234",
            "2001 3 14","1234567","099555555",
            "manoloreal@gmail.com",new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        Administrador admin = registryHandler.RegistrarAdministrador("elpepeAdmin", "1234", "1234", "god@dog.com");
        Categoria categoria = ofertasHandler.CrearCategoria(admin,"Tareas del hogar");

        // Act
        OfertaDeServicio oferta = ofertasHandler.Ofertar(categoria.GetId(),(Trabajador)miUsuario,"El mejor limpiador de Salto","Limpiador",9000);
        OfertaDeServicio expected = oferta;
        OfertaDeServicio result = ofertasHandler.GetOfertaById(oferta.GetId());
        
        // Assert
        Assert.That(expected.Equals(result));
    }

    [Test]
    public void EmpleadorRegistrarse()
    // Como empleador, quiero registrarme en la plataforma, indicando mis datos personales e información de contacto
    // para que de esa forma, pueda proveer información de contacto a los trabajadores que quiero contratar.
    {
        // Arrange
        RegistryHandler registryHandler = RegistryHandler.GetInstance();
        bool expected = true;
        
        // Act
        Usuario miUsuario = registryHandler.RegistrarEmpleador("Señor Manolo","Manolete", "BigManoler","1234",
            "1970 3 14","1234567","099555555",
            "mistermanoloreal@gmail.com",new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        bool result = (miUsuario is Empleador);
        
        // Assert
        Assert.That(result.Equals(expected));
        
    }

    [Test]
    public void EmpleadorBuscarOfertaPorCategoria()
    // Como empleador, quiero buscar ofertas de trabajo, opcionalmente filtrando por categoría para que de esa forma,
    // pueda contratar un servicio.
    {
        // Arrange
        RegistryHandler registryHandler = RegistryHandler.GetInstance();
        OfertasHandler ofertasHandler = OfertasHandler.GetInstance();
        Administrador adm= registryHandler.RegistrarAdministrador("admin", "toor", "1234", "aaa@aaa.com");
        SearchHandler searchHandler = new();
        Categoria cat = ofertasHandler.CrearCategoria(adm, "categoria");
        bool expected = true;
        
        // Act
        // TODO buscador, cambiar el new por el return del buscador
        List<OfertaDeServicio> ofertasFiltradasPorCategoria = searchHandler.FiltrarCategoria(cat);
        Categoria categoriaAgregada = ofertasHandler.GetCategoriaById(cat.GetId());
        bool result = true; // result es true a no ser que el contenido de la categoria no sea el mismo que el retornado por el metodo del buscador por categoria
        
        foreach (OfertaDeServicio ofertaDeServicio in ofertasFiltradasPorCategoria)
        {
            if (!categoriaAgregada.GetOfertas().Contains(ofertaDeServicio)) result = false;
        }

        foreach (OfertaDeServicio ofertaDeServicio in categoriaAgregada.GetOfertas())
        {
            if (!cat.GetOfertas().Contains(ofertaDeServicio)) result = false;
        }

        // Assert
        Assert.That(result.Equals(expected));

    }

    [Test]
    public void EmpleadorBuscarOfertaPorDistancia()
    // Como empleador, quiero ver el resultado de las búsquedas de ofertas de trabajo ordenado en forma ascendente de
    // distancia a mi ubicación, es decir, las más cercanas primero para que de esa forma, pueda poder contratar un
    // servicio.
    {
        // Arrange
        RegistryHandler registryHandler = RegistryHandler.GetInstance();
        SearchHandler searchHandler = new();
        OfertasHandler ofertasHandler = OfertasHandler.GetInstance();
        Administrador admin = registryHandler.RegistrarAdministrador("admin", "root", "1234", "abc@abc.abc");
        Empleador empleador = registryHandler.RegistrarEmpleador("a", "a", "a", "a", "2020 1 1", "1234567", "123",
            "alb@aa.asd", new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        Categoria cat = ofertasHandler.CrearCategoria(admin, "Categoria");
        bool expected = true;

        
        // Act
        // TODO buscador, cambiar el new por el return del metodo que sea y borrar el Assert.Pass
        List<OfertaDeServicio> ofertasFiltradasPorUbicacion = searchHandler.FiltrarDistancia(empleador);
        Assert.Pass();
        double distanciaAnterior = 0;
        double distanciaActual = 0;
        int i = 0;
        bool result = true; // result es true a no ser que en algun punto lista(n+1) sea menor que lista(n)
        foreach (OfertaDeServicio ofertaDeServicio in ofertasFiltradasPorUbicacion)
        {
            distanciaAnterior = distanciaActual;
            i++;
            double latO = ofertaDeServicio.GetUbicacion().Item1;
            double longO = ofertaDeServicio.GetUbicacion().Item2;
            double latE = empleador.Ubicacion.Item1;
            double longE = empleador.Ubicacion.Item2;
            double term1 = Math.Pow((latE - latO), 2);
            double term2 = Math.Pow((longE - longO), 2);
            distanciaActual = Math.Sqrt(term1+term2);

            if (i <= 1) continue;
            if (distanciaActual < distanciaAnterior) result = false;
        }

        // Assert
        Assert.That(expected.Equals(result));
        
    }

    [Test]
    public void EmpleadorBuscarOfertaPorReputacion()
    // Como empleador, quiero ver el resultado de las búsquedas de ofertas de trabajo ordenado en forma descendente por
    // reputación, es decir, las de mejor reputación primero para que de esa forma, pueda contratar un servicio.
    {
        
        // Arrange
        RegistryHandler registryHandler = RegistryHandler.GetInstance();
        OfertasHandler ofertasHandler = OfertasHandler.GetInstance();
        Administrador admin = registryHandler.RegistrarAdministrador("admin", "toor", "1234", "ada@aa.aa");
        Categoria cat = ofertasHandler.CrearCategoria(admin, "categoria");
        SearchHandler searchHandler = new();
        bool expected = true;
        
        // Act
        List<OfertaDeServicio> OfertasFiltradasPorReputacion = searchHandler.FiltrarReputacion();
        Calificacion reputacionAnterior = 0;
        Calificacion reputacionActual = 0;
        bool result = true; // result es true a no ser que en algun la reputacion de list(n+1) sea mayor que la de list(n)
        int i = 0;
        foreach (OfertaDeServicio ofertaDeServicio in OfertasFiltradasPorReputacion)
        {
            i++;
            reputacionAnterior = reputacionActual;

            reputacionActual = ofertaDeServicio.GetReputacion();
            
            if (i <= 1) continue;
            if (reputacionActual > reputacionAnterior) result = false;
        }

        // Assert
        Assert.That(result.Equals(expected));
        
    }

    [Test]
    public void EmpleadorContactarTrabajador()
    // Como empleador, quiero poder contactar a un trabajador para que de esa forma pueda, contratar una oferta de
    // servicio determinada.
    {

        // Arrange
        RegistryHandler registryHandler = RegistryHandler.GetInstance();
        OfertasHandler ofertasHandler = OfertasHandler.GetInstance();
        ContratoHandler contratoHandler = ContratoHandler.GetInstance();
        Administrador admin = registryHandler.RegistrarAdministrador("admin", "toor", "1234", "administrado@gmail.com");
        Categoria cat = ofertasHandler.CrearCategoria(admin, "categoria");
        Trabajador pepe = registryHandler.RegistrarTrabajador("a", "a", "a", "a", "2020,2,2", "1234556", "12345", "aaa@sda.ssa",
            new Tuple<double, double>(1, 1));
        OfertaDeServicio oferta = ofertasHandler.Ofertar(cat.GetId(), pepe ,"soy pro", "gamer", 10);
        Empleador mrbossman = registryHandler.RegistrarEmpleador("mr", "bossman", "eljefe", "lospoios", "2010,10,10",
            "1234567", "1234", "gus@lospoiosermanos.com", new Tuple<double, double>(10, 10));
        OfertaDeServicio expected = oferta;
        
        // Act
        contratoHandler.SolicitarTrabajador(oferta,mrbossman);
        OfertaDeServicio result = contratoHandler.GetSolicitudes(pepe).FirstOrDefault().Oferta;

        // Assert
        Assert.That(expected.Equals(result));
    }

    [Test]
    public void TrabajadorCalificarEmpleador1()
    // Como trabajador, quiero poder calificar a un empleador; el empleador me tiene que calificar a mí también, si no
    // me califica en un mes, la calificación será neutral, para que de esa forma pueda definir la reputación de mi
    // empleador.
    // Empleador califica a trabajador.
    {

        // Arrange
        RegistryHandler registryHandler = RegistryHandler.GetInstance();
        OfertasHandler ofertasHandler = OfertasHandler.GetInstance();
        ContratoHandler contratoHandler = ContratoHandler.GetInstance();
        Trabajador trabajador = registryHandler.RegistrarTrabajador("TNombre", "TApellido", "TNick", "TPass",
            "1970 1 1", "1234567",
            "473555555", "trabajador@dominio.com", new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        Administrador admin = registryHandler.RegistrarAdministrador("Admin", "toor", "473555555", "admin@dominio.com");
        Empleador empleador = registryHandler.RegistrarEmpleador("ENombre", "EApellido", "ENick", "EPass", "1970 1 1",
            "1234567",
            "473555555", "empleador@dominio.com", new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        Categoria categoria = ofertasHandler.CrearCategoria(admin, "Categoria");
        OfertaDeServicio ofertaDeServicio = ofertasHandler.Ofertar(categoria.GetId(), trabajador, "Descripcion", "Empleo", 1000);
        Calificacion expected = Calificacion.Sobresaliente;
        
        // Act
        Solicitud solicitud = contratoHandler.SolicitarTrabajador(ofertaDeServicio, empleador);
        contratoHandler.AceptarSolicitud(trabajador, solicitud);
        solicitud = contratoHandler.GetSolicitud(solicitud.GetId());
        solicitud.CalificarTrabajador(empleador,Calificacion.Sobresaliente);
        Calificacion result = solicitud.GetTrabajadorRate();

        // Assert
        Assert.That(result.Equals(expected));
    }
    
    [Test]
    public void TrabajadorCalificarEmpleador2()
        // Como trabajador, quiero poder calificar a un empleador; el empleador me tiene que calificar a mí también, si no
        // me califica en un mes, la calificación será neutral, para que de esa forma pueda definir la reputación de mi
        // empleador.
        // Trabajador califica a empleador.
    {
        // Arrange
        RegistryHandler registryHandler = RegistryHandler.GetInstance();
        OfertasHandler ofertasHandler = OfertasHandler.GetInstance();
        ContratoHandler contratoHandler = ContratoHandler.GetInstance();
        Trabajador trabajador = registryHandler.RegistrarTrabajador("TNombre", "TApellido", "TNick", "TPass",
            "1970 1 1", "1234567",
            "473555555", "trabajador@dominio.com", new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        Administrador admin = registryHandler.RegistrarAdministrador("Admin", "toor", "473555555", "admin@dominio.com");
        Empleador empleador = registryHandler.RegistrarEmpleador("ENombre", "EApellido", "ENick", "EPass", "1970 1 1",
            "1234567",
            "473555555", "empleador@dominio.com", new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        Categoria categoria = ofertasHandler.CrearCategoria(admin, "Categoria");
        OfertaDeServicio ofertaDeServicio = ofertasHandler.Ofertar(categoria.GetId(), trabajador, "Descripcion", "Empleo", 1000);
        Calificacion expected = Calificacion.Sobresaliente;
        
        // Act
        Solicitud solicitud = contratoHandler.SolicitarTrabajador(ofertaDeServicio, empleador);
        contratoHandler.AceptarSolicitud(trabajador, solicitud);
        solicitud = contratoHandler.GetSolicitud(solicitud.GetId());
        solicitud.CalificarEmpleador(trabajador,Calificacion.Sobresaliente);
        Calificacion result = solicitud.GetEmpleadorRate();

        // Assert
        Assert.That(result.Equals(expected));
        
    }
    
    [Test]
    public void TrabajadorCalificarEmpleador3()
        // Como trabajador, quiero poder calificar a un empleador; el empleador me tiene que calificar a mí también, si no
        // me califica en un mes, la calificación será neutral, para que de esa forma pueda definir la reputación de mi
        // empleador.
        // Empleador NO califica a trabajador tras un mes.
    {
        // Arrange
        RegistryHandler registryHandler = RegistryHandler.GetInstance();
        OfertasHandler ofertasHandler = OfertasHandler.GetInstance();
        ContratoHandler contratoHandler = ContratoHandler.GetInstance();
        Trabajador trabajador = registryHandler.RegistrarTrabajador("TNombre", "TApellido", "TNick", "TPass",
            "1970 1 1", "1234567",
            "473555555", "trabajador@dominio.com", new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        Administrador admin = registryHandler.RegistrarAdministrador("Admin", "toor", "473555555", "admin@dominio.com");
        Empleador empleador = registryHandler.RegistrarEmpleador("ENombre", "EApellido", "ENick", "EPass", "1970 1 1",
            "1234567",
            "473555555", "empleador@dominio.com", new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        Categoria categoria = ofertasHandler.CrearCategoria(admin, "Categoria");
        OfertaDeServicio ofertaDeServicio = ofertasHandler.Ofertar(categoria.GetId(), trabajador, "Descripcion", "Empleo", 1000);
        Calificacion expected = Calificacion.Bueno;

        // Act
        Solicitud solicitud = contratoHandler.SolicitarTrabajador(ofertaDeServicio, empleador);
        contratoHandler.AceptarSolicitud(trabajador, solicitud);
        solicitud = contratoHandler.GetSolicitud(solicitud.GetId());
        solicitud.CalificarEmpleador(trabajador,Calificacion.Sobresaliente);
        Updater.FakeUpdate(DateTime.Now.Add(new TimeSpan(31,0,0,0)));
        Calificacion result = solicitud.GetTrabajadorRate();

        // Assert
        Assert.That(result.Equals(expected));
        
    }

    [Test]
    public void TrabajadorCalificarEmpleador4()
    // Como empleador, quiero poder calificar a un trabajador; el trabajador me tiene que calificar a mí también, si no
    // me califica en un mes, la calificación será neutral, para que de esa forma, pueda definir la reputación del
    // trabajador.
    // Trabajador NO califica a empleador tras un mes.

    {
        // Arrange
        RegistryHandler registryHandler = RegistryHandler.GetInstance();
        OfertasHandler ofertasHandler = OfertasHandler.GetInstance();
        ContratoHandler contratoHandler = ContratoHandler.GetInstance();
        Trabajador trabajador = registryHandler.RegistrarTrabajador("TNombre", "TApellido", "TNick", "TPass",
            "1970 1 1", "1234567",
            "473555555", "trabajador@dominio.com", new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        Administrador admin = registryHandler.RegistrarAdministrador("admin", "toor", "1234", "aasds@aasda.aco");
        Empleador empleador = registryHandler.RegistrarEmpleador("ENombre", "EApellido", "ENick", "EPass", "1970 1 1",
            "1234567",
            "473555555", "empleador@dominio.com", new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        Categoria categoria = ofertasHandler.CrearCategoria(admin, "Categoria");
        OfertaDeServicio ofertaDeServicio = ofertasHandler.Ofertar(categoria.GetId(), trabajador, "Descripcion", "Empleo", 1000);
        Calificacion expected = Calificacion.Bueno;
        
        // Act
        Solicitud solicitud = contratoHandler.SolicitarTrabajador(ofertaDeServicio, empleador);
        contratoHandler.AceptarSolicitud(trabajador, solicitud);
        solicitud = contratoHandler.GetSolicitud(solicitud.GetId());
        solicitud.CalificarTrabajador(empleador,Calificacion.Sobresaliente);
        Updater.FakeUpdate(DateTime.Now.Add(new TimeSpan(31,0,0,0)));
        Calificacion result = solicitud.GetEmpleadorRate();

        // Assert
        Assert.That(result.Equals(expected));
        
    }

    [Test]
    public void TrabajadorVerReputacionEmpleador()
        // Como trabajador, quiero poder saber la reputación de un empleador que me contacte para que de esa forma, poder
        // decidir sobre su solicitud de contratación.
    {
        // Arrange
        RegistryHandler registryHandler = RegistryHandler.GetInstance();
        ContratoHandler contratoHandler = ContratoHandler.GetInstance();
        OfertasHandler ofertasHandler = OfertasHandler.GetInstance();
        Administrador admin = registryHandler.RegistrarAdministrador("Admin", "toor", "473555555", "admin@dominio.com");
        Empleador empleador = registryHandler.RegistrarEmpleador("ENombre", "EApellido", "ENick", "EPass", "1970 1 1",
            "1234567",
            "473555555", "empleador@dominio.com", new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        Trabajador trabajador = registryHandler.RegistrarTrabajador("TNombre", "TApellido", "TNick", "TPass",
            "1970 1 1", "1234567",
            "473555555", "trabajador@dominio.com", new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        Categoria categoria = ofertasHandler.CrearCategoria(admin, "Categoria");
        OfertaDeServicio ofertaDeServicio = ofertasHandler.Ofertar(categoria.GetId(), trabajador, "Descripcion", "Empleo", 1000);
        Calificacion expected = Calificacion.NoCalificado;

        
        // Act
        Solicitud solicitud = contratoHandler.SolicitarTrabajador(ofertaDeServicio, empleador);
        Calificacion result = registryHandler.GetReputacion(solicitud.GetEmpleador());


        // Assert
        Assert.That(result.Equals(expected));
        
    }

    [Test]
    public void TrabajadorAceptaSolicitud()
    // El trabajador crea una oferta de servicio, el empleador la solicita y el trabajador acepta esa solicitud 
    {
        // Arrange
        
        // Obtenemos las instancias de los handlers del core
        RegistryHandler registryHandler = RegistryHandler.GetInstance();
        ContratoHandler contratoHandler = ContratoHandler.GetInstance();
        OfertasHandler ofertasHandler = OfertasHandler.GetInstance();
        // Creamos un administrador para que cree una categoria
        Administrador admin = registryHandler.RegistrarAdministrador("root", "toor", "123", "aa@aa.co");
        Categoria cat = ofertasHandler.CrearCategoria(admin, "Categoria");
        // Creamos el trabajador y empleador que van a interactuar
        Trabajador t1 = registryHandler.RegistrarTrabajador("Pepe", "Pepito", "Elpepe", "1234", "1 10 2000", "54204115", "473555555",
            "correo@valido.com", new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        Empleador e1 = registryHandler.RegistrarEmpleador("Juan", "Sech", "Etesech", "4321", "1 11 1197", "58594115", "472555555",
            "correo@confiable.net", new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        // El trabajador crea una oferta
        OfertaDeServicio oferta = ofertasHandler.Ofertar(cat.GetId(), t1, "un trabajo", "trabajado", 10);
        // El empleador la solicita
        Solicitud solicitud = contratoHandler.SolicitarTrabajador(oferta, e1);
        bool expected = false;
        
        // Act
        
        // El trabajador acepta la solicitud
        contratoHandler.AceptarSolicitud(t1,solicitud);
        bool result = oferta.Disponible;
        
        // Assert
        Assert.That(expected.Equals(result));

    }
    
    [Test]
    public void TrabajadorNoAceptaSolicitud()
        // El trabajador crea una oferta de servicio, el empleador la solicita y el trabajador no acepta esa solicitud 
    {
        // Arrange
        
        // Obtenemos las instancias de los handlers del core
        RegistryHandler registryHandler = RegistryHandler.GetInstance();
        ContratoHandler contratoHandler = ContratoHandler.GetInstance();
        OfertasHandler ofertasHandler = OfertasHandler.GetInstance();
        // Creamos un administrador para que cree una categoria
        Administrador admin = registryHandler.RegistrarAdministrador("root", "toor", "123", "aa@aa.co");
        Categoria cat = ofertasHandler.CrearCategoria(admin, "Categoria");
        // Creamos el trabajador y empleador que van a interactuar
        Trabajador t1 = registryHandler.RegistrarTrabajador("Pepe", "Pepito", "Elpepe", "1234", "1 10 2000", "54204115", "473555555",
            "correo@valido.com", new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        Empleador e1 = registryHandler.RegistrarEmpleador("Juan", "Sech", "Etesech", "4321", "1 11 1197", "58594115", "472555555",
            "correo@confiable.net", new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        // El trabajador crea una oferta
        OfertaDeServicio oferta = ofertasHandler.Ofertar(cat.GetId(), t1, "un trabajo", "trabajado", 10);
        // El empleador la solicita
        Solicitud solicitud = contratoHandler.SolicitarTrabajador(oferta, e1);
        bool expected = true;
        
        // Act
        
        // El trabajador acepta la solicitud
        contratoHandler.RechazarSolicitud(t1,solicitud);
        bool result = oferta.Disponible;
        
        // Assert
        Assert.That(expected.Equals(result));

    }
    
    // Test de que el trabajador crea una oferta de servicio, un empleador la solicita y el trabajador acepta la solicitud. Después otro empleador
    // la intenta solicitar
    private void ErrorEmpleadorSolicitaOfertaNoDisponible()
    {
        // Arrange 
        
        // Obtenemos las instancias de los handlers del core
        RegistryHandler registryHandler = RegistryHandler.GetInstance();
        ContratoHandler contratoHandler = ContratoHandler.GetInstance();
        OfertasHandler ofertasHandler = OfertasHandler.GetInstance();
        // Creamos un administrador para que cree una categoria
        Administrador admin = registryHandler.RegistrarAdministrador("root", "toor", "123", "aa@aa.co");
        Categoria cat = ofertasHandler.CrearCategoria(admin, "Categoria");
        // Creamos el trabajador y los empleadores que van a interactuar
        Trabajador t1 = registryHandler.RegistrarTrabajador("Pepe", "Pepito", "Elpepe", "1234", "1 10 2000", "54204115", "473555555",
            "correo@valido.com", new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        Empleador e1 = registryHandler.RegistrarEmpleador("Juan", "Sech", "Etesech", "4321", "1 11 1197", "58594115", "472555555",
            "correo@confiable.net", new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        Empleador e2 = registryHandler.RegistrarEmpleador("OtroJuan", "Sech", "Etesech2", "4321", "1 11 1197", "58594115", "472555555",
            "correo@correo.net", new Tuple<double, double>(-31.389425985682045, -57.959432913914476));
        // El trabajador crea una oferta
        OfertaDeServicio oferta = ofertasHandler.Ofertar(cat.GetId(), t1, "un trabajo", "trabajado", 10);
        // El empleador la solicita y el trabajador acepta
        Solicitud solicitud = contratoHandler.SolicitarTrabajador(oferta, e1);
        contratoHandler.AceptarSolicitud(t1,solicitud);
        
        // Act
        
        // El segundo empleador solicita la oferta ya aceptada
        contratoHandler.SolicitarTrabajador(oferta, e2);
    }
    
    [Test]
    public void EmpleadorSolicitaOfertaNoDisponible()
    // continuacion del test
    {
        
        // Assert
        Assert.Throws<Exception>(ErrorEmpleadorSolicitaOfertaNoDisponible);

    }
    
    
}