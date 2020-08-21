namespace Sample.Testes.API.Features.v1.User.Model
{
    public class UserModel
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public int Idade { get; private set; }

        protected UserModel() {}

        public UserModel(int id, string nome, int idade)
        {
            Id = id;
            Nome = nome;
            Idade = idade;
        }
    }
}

