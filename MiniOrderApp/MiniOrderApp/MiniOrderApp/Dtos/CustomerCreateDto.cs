namespace MiniOrderApp.Dtos;


public class CustomerCreateDto
{
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
}