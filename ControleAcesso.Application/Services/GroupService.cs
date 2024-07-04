using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Interfaces.Services;
using ControleAcesso.Domain.Interfaces.Repositories;
using ControleAcesso.Domain.Exceptions;
using ControleAcesso.Domain.Enumerations;


namespace ControleAcesso.Application.Services
{
    public class GroupService : GenericService<GroupAd>, IGroupService
    {
        private readonly IGroupRespository _groupRepository;
        public GroupService(IGenericRepository<GroupAd> repository, IGroupRespository groupRepository) : base(repository)
        {
            _groupRepository = groupRepository;
        }

        public override GroupAd Add(GroupAd entity)
        {
            //Antes de adicionar, deve ser validado se o grupo do AD informado no DN existe, e verificar se ja existe algum registro com o DN informado
            var validationErrors = Validate(entity);
            if (validationErrors.Any())
            {
                throw new GroupValidateException("Erro de validação do Grupo, verifique se foi preenchido corretamente",validationErrors);
            }
            return base.Add(entity);
        }

        public override async Task<IEnumerable<GroupAd>> GetAllAsync(NavigationLevel navigationLevel = NavigationLevel.None)
        {
            return await _groupRepository.GetAllGroupAdAsync();
        }

        public async override Task<GroupAd> UpdateAsync(GroupAd entity)
        {
            var validationErrors = Validate(entity);
            if (!await isGroupExists(entity))
            {
                validationErrors.Add("Grupo informado não existe na base de dados");   
            }

            if (validationErrors.Any())
            {
                throw new GroupValidateException("Erro de validação do Grupo, verifique se foi preenchido corretamente", validationErrors);
            }

            return await base.UpdateAsync(entity);
        }

        private List<string> Validate(GroupAd entity)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(entity.Name) || entity.Name.Length > 100)
                errors.Add("O nome é obrigatório e deve ter no máximo 100 caracteres.");

            if (string.IsNullOrWhiteSpace(entity.Dn) || entity.Dn.Length > 100)
                errors.Add("O DN é obrigatório e deve ter no máximo 100 caracteres.");

            if (entity.ExpiresHour.HasValue && (entity.ExpiresHour < 24 || entity.ExpiresHour > 72))
                errors.Add("O campo ExpiresHour deve estar entre 24 e 72.");

            return errors;
        }

        private Dictionary<string, List<string>> Validatee(GroupAd entity)
        {
            var errors = new Dictionary<string, List<string>>();

            void AddError(string propertyName, string errorMessage)
            {
                if (!errors.ContainsKey(propertyName))
                    errors[propertyName] = new List<string>();
                errors[propertyName].Add(errorMessage);
            }

            if (string.IsNullOrWhiteSpace(entity.Name))
                AddError(nameof(entity.Name), "O nome é obrigatório.");
            if (!string.IsNullOrWhiteSpace(entity.Name) && entity.Name.Length > 100)
                AddError(nameof(entity.Name), "O nome deve ter no máximo 100 caracteres.");

            if (string.IsNullOrWhiteSpace(entity.Dn))
                AddError(nameof(entity.Dn), "O DN é obrigatório.");
            if (!string.IsNullOrWhiteSpace(entity.Dn) && entity.Dn.Length > 100)
                AddError(nameof(entity.Dn), "O DN deve ter no máximo 100 caracteres.");

            if (entity.ExpiresHour.HasValue)
            {
                if (entity.ExpiresHour < 24 || entity.ExpiresHour > 72)
                    AddError(nameof(entity.ExpiresHour), "O campo ExpiresHour deve estar entre 24 e 72.");
            }

            return errors;
        }

        private Boolean IsExistGroupAD(GroupAd entity)
        {
            //Faz a consulta LDAP para verificar se o DN informado e válido
            return true;
        }

        private async Task<Boolean> isGroupExists(GroupAd entity)
        {
            var group = await _repository.GetAsync(e => e.Id == entity.Id);
            if (group != null) 
            {
                return true;
            }
            return false;
        }

       
    }
}
