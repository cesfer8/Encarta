using Encarta.Models;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Encarta.Services
{
    public static class DataBaseControl
    {
        public static async Task<bool> RegisterUsuario(Usuario usuario, string contrasenia)
        {
            try
            {
                //AUTH
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(App.APIKEY));
                var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(usuario.Correo, contrasenia);
                //DB
                FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
                var data = await firebaseClient.Child("Usuarios").PostAsync(JsonConvert.SerializeObject(usuario));
                if (!string.IsNullOrEmpty(data.Key))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }

        public static async Task<bool> AddCartaFavUsuario(string idUsuario, string cartaFavId)
        {
            if(idUsuario == "none")
            {
                return false;
            }
            try
            {
                Usuario usuario = await GetUsuario(idUsuario);
                if(usuario == null)
                {
                    return false;
                }
                List<string> listadoCartas = new List<string>();
                if(usuario.CartasFavoritas != null)
                {
                    listadoCartas = usuario.CartasFavoritas;
                }
                listadoCartas.Add(cartaFavId);

                FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
                await firebaseClient.Child("Usuarios").Child(idUsuario).Child("CartasFavoritas").PutAsync(JsonConvert.SerializeObject(listadoCartas));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }

        public static async Task<bool> RemoveCartaFavUsuario(string idUsuario, string cartaFavId)
        {
            if (idUsuario == "none")
            {
                return false;
            }
            try
            {
                Usuario usuario = await GetUsuario(idUsuario);
                if (usuario == null)
                {
                    return false;
                }
                List<string> listadoCartas = new List<string>();
                if (usuario.CartasFavoritas != null)
                {
                    listadoCartas = usuario.CartasFavoritas;
                }
                listadoCartas.Remove(cartaFavId);

                FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
                await firebaseClient.Child("Usuarios").Child(idUsuario).Child("CartasFavoritas").PutAsync(JsonConvert.SerializeObject(listadoCartas));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }

        public static async Task<bool> RemoveCarta(string idCarta)
        {
            try
            {
                FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
                await firebaseClient.Child("Cartas").Child(idCarta).DeleteAsync();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }

        public static async Task<bool> CheckUsuarioLogged(Usuario usuario, string contrasenia)
        {
            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(App.APIKEY));
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(usuario.Correo, contrasenia);
                //var content = await auth.GetFreshAuthAsync();
                //var serializedcontnet = JsonConvert.SerializeObject(content);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }

        public static async Task<Usuario> GetUsuarioEmail(string correo)
        {
            FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
            return (await firebaseClient.Child("Usuarios").OnceAsync<Usuario>()).Select(item => new Usuario
            {
                Id = item.Key,
                Correo = item.Object.Correo,
                Nombre = item.Object.Nombre,
                CartasFavoritas = item.Object.CartasFavoritas,
                Admin = item.Object.Admin,
                Bloqueado = item.Object.Bloqueado
            }).Where(usuario => usuario.Correo == correo).FirstOrDefault();
        }

        public static async Task<Usuario> GetUsuario(string id)
        {
            FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
            return (await firebaseClient.Child("Usuarios").OnceAsync<Usuario>()).Select(item => new Usuario
            {
                Id = item.Key,
                Correo = item.Object.Correo,
                Nombre = item.Object.Nombre,
                CartasFavoritas = item.Object.CartasFavoritas,
                Admin = item.Object.Admin,
                Bloqueado = item.Object.Bloqueado
            }).Where(usuario => usuario.Id == id).FirstOrDefault();
        }

        public static async Task<List<Usuario>> GetAllUsuarios()
        {
            FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
            return (await firebaseClient.Child("Usuarios").OnceAsync<Usuario>()).Select(item => new Usuario
            {
                Id = item.Key,
                Correo = item.Object.Correo,
                Nombre = item.Object.Nombre,
                CartasFavoritas = item.Object.CartasFavoritas,
                Admin = item.Object.Admin,
                Bloqueado = item.Object.Bloqueado
            }).ToList();
        }

        //Devuelve key de restaurante si solo se añade restaurante o key de carta si se añade carta tambien
        public static async Task<string> AddRestaurante(Restaurante restauranteNuevo, Carta c = null)
        {
            FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
            var data = await firebaseClient.Child("Restaurantes").PostAsync(JsonConvert.SerializeObject(restauranteNuevo));
            if (string.IsNullOrEmpty(data.Key))
            {
                return String.Empty;
            }
            if (c == null)
            {
                return data.Key;
            }
            return await AddCartaToRestaurante(c, data.Key);
        }

        //devuelve url de la imagen
        public static async Task<string> AddCartaImagen(Stream imageStream, string fileName) 
        {
            try
            {
                if (fileName.Contains("/"))
                {
                    fileName = fileName.Split(Path.DirectorySeparatorChar).Last();
                }

                var urlImage = await new FirebaseStorage(App.DBSTORAGE,
                    new FirebaseStorageOptions
                    {
                        ThrowOnCancel = true
                    })
                .Child("Cartas")
                .Child(fileName)
                .PutAsync(imageStream);
                return urlImage;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }

        //Devuelve key de la carta añadida o cadena vacia si no ha sido posible
        public static async Task<string> AddCartaToRestaurante(Carta cartaNueva, string restauranteId = null)
        {
            if (restauranteId != null)
            {
                cartaNueva.Id_restaurante = restauranteId;
            }

            FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
            var data = await firebaseClient.Child("Cartas").PostAsync(JsonConvert.SerializeObject(cartaNueva));
            if (!string.IsNullOrEmpty(data.Key))
            {
                return data.Key;
            }
            return String.Empty;
        }

        public static async Task<List<Carta>> GetCartasFromRestaurante(string restauranteId)
        {
            FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
            return (await firebaseClient.Child("Cartas").OnceAsync<Carta>()).Select(item => new Carta
            {
                Id = item.Key,
                Fecha_creacion = item.Object.Fecha_creacion,
                Id_restaurante = item.Object.Id_restaurante,
                Id_usuario = item.Object.Id_usuario,
                UrlQR = item.Object.UrlQR,
                Foto = item.Object.Foto
            }).Where(carta => carta.Id_restaurante == restauranteId).ToList();
        }

        public static async Task<List<Carta>> GetCartasFromUsuario(string usuarioId)
        {
            FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
            return (await firebaseClient.Child("Cartas").OnceAsync<Carta>()).Where(carta => carta.Object.Id_usuario == usuarioId).Select(item => new Carta
            {
                Id = item.Key,
                Fecha_creacion = item.Object.Fecha_creacion,
                Id_restaurante = item.Object.Id_restaurante,
                Id_usuario = item.Object.Id_usuario,
                UrlQR = item.Object.UrlQR,
                Foto = item.Object.Foto
            }).ToList();
        }

        public static async Task<Carta> GetCartaById(string cartaId)
        {
            FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
            return (await firebaseClient.Child("Cartas").OnceAsync<Carta>()).Select(item => new Carta
            {
                Id = item.Key,
                Fecha_creacion = item.Object.Fecha_creacion,
                Id_restaurante = item.Object.Id_restaurante,
                Id_usuario = item.Object.Id_usuario,
                UrlQR = item.Object.UrlQR,
                Foto = item.Object.Foto,
                Denuncias = item.Object.Denuncias
            }).Where(carta => carta.Id == cartaId).FirstOrDefault();
        }

        public static async Task<List<Carta>> GetCartasFav(string usuarioId)
        {
            FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
            Usuario u = await GetUsuario(usuarioId);
            if(u.CartasFavoritas == null)
            {
                return new List<Carta>();
            }

            return (await firebaseClient.Child("Cartas").OnceAsync<Carta>()).Where(carta => u.CartasFavoritas.Contains(carta.Key)).Select(item => new Carta
            {
                Id = item.Key,
                Fecha_creacion = item.Object.Fecha_creacion,
                Id_restaurante = item.Object.Id_restaurante,
                Id_usuario = item.Object.Id_usuario,
                UrlQR = item.Object.UrlQR,
                Foto = item.Object.Foto
            }).ToList();
        }

        public static async Task<List<Carta>> GetAllCartas()
        {
            FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
            return (await firebaseClient.Child("Cartas").OnceAsync<Carta>()).Select(item => new Carta
            {
                Id = item.Key,
                Fecha_creacion = item.Object.Fecha_creacion,
                Id_restaurante = item.Object.Id_restaurante,
                Id_usuario = item.Object.Id_usuario,
                UrlQR = item.Object.UrlQR,
                Foto = item.Object.Foto
            }).ToList();
        }

        public static async Task<List<Restaurante>> GetAllRestaurantes()
        {
            FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
            return (await firebaseClient.Child("Restaurantes").OnceAsync<Restaurante>()).Select(item => new Restaurante
            {
                Id = item.Key,
                Nombre = item.Object.Nombre,
                Latitude = item.Object.Latitude,
                Longitude = item.Object.Longitude,
            }).ToList();
        }

        public static async Task<Restaurante> GetRestaurantesFromId(string idRestaurante)
        {
            FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
            return (await firebaseClient.Child("Restaurantes").OnceAsync<Restaurante>()).Select(item => new Restaurante
            {
                Id = item.Key,
                Nombre = item.Object.Nombre,
                Latitude = item.Object.Latitude,
                Longitude = item.Object.Longitude
            }).Where(restaurante => restaurante.Id == idRestaurante).FirstOrDefault();
        }

        public static async Task<bool> AddDenunciaCarta(string idCarta)
        {
            try
            {
                FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
                Carta carta = await GetCartaById(idCarta);
                carta.Id = null;
                carta.Denuncias += 1;
                await firebaseClient.Child("Cartas").Child(idCarta).PutAsync(JsonConvert.SerializeObject(carta));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }

        public static async Task<bool> RemoveDenunciasCarta(string idCarta)
        {
            try
            {
                FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
                Carta carta = await GetCartaById(idCarta);
                carta.Id = null;
                carta.Denuncias = 0;
                await firebaseClient.Child("Cartas").Child(idCarta).PutAsync(JsonConvert.SerializeObject(carta));
                return true;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }

        public static async Task<List<Carta>> GetCartasWithDenuncias()
        {
            FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
            return (await firebaseClient.Child("Cartas").OnceAsync<Carta>()).Select(item => new Carta
            {
                Id = item.Key,
                Fecha_creacion = item.Object.Fecha_creacion,
                Id_restaurante = item.Object.Id_restaurante,
                Id_usuario = item.Object.Id_usuario,
                UrlQR = item.Object.UrlQR,
                Foto = item.Object.Foto,
                Denuncias = item.Object.Denuncias
            }).Where(carta => carta.Denuncias>0).OrderByDescending(carta => carta.Denuncias).ToList();
        }

        public static async Task<bool> AddAdminToUser(string correo)
        {
            try
            {
                FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
                Usuario usuario = await GetUsuarioEmail(correo);
                if(usuario == null)
                {
                    return false;
                }
                string usuarioId = usuario.Id;
                usuario.Id = null;
                usuario.Admin = true;
                await firebaseClient.Child("Usuarios").Child(usuarioId).PutAsync(JsonConvert.SerializeObject(usuario));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }

        public static async Task<bool> BlockUsuario(string idUsuario)
        {

            try
            {
                FirebaseClient firebaseClient = new FirebaseClient(App.DBURL);
                Usuario usuario = await GetUsuario(idUsuario);
                if (usuario == null)
                {
                    return false;
                }
                usuario.Id = null;
                usuario.Bloqueado = true;
                await firebaseClient.Child("Usuarios").Child(idUsuario).PutAsync(JsonConvert.SerializeObject(usuario));
                return true;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }
    }
}
