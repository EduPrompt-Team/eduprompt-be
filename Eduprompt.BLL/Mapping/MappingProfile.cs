using AutoMapper;
using Eduprompt.Domain.DTOs.Auth;
using Eduprompt.Domain.DTOs.Cart;
using Eduprompt.Domain.DTOs.Order;
using Eduprompt.Domain.DTOs.Role;
using Eduprompt.Domain.DTOs.StorageTemplate;
using Eduprompt.Domain.DTOs.User;
using Eduprompt.Domain.DTOs.Wishlist;
using Eduprompt.Domain.DTOs.Package;
using Eduprompt.Domain.DTOs.PackageCategory;
using Eduprompt.Domain.DTOs.PackageDetail;
using Eduprompt.Domain.DTOs.APIKey;
using Eduprompt.Domain.DTOs.PromptInstance;
using Eduprompt.Domain.DTOs.PromptInstanceDetail;
using Eduprompt.Domain.DTOs.TemplateArchitecture;
using Eduprompt.Domain.DTOs.Conversation;
using Eduprompt.Domain.DTOs.Message;
using Eduprompt.Domain.DTOs.PaymentMethod;
using Eduprompt.Domain.DTOs.Transaction;
using Eduprompt.Domain.DTOs.Wallet;
using Eduprompt.Domain.DTOs.Post;
using Eduprompt.Domain.DTOs.Feedback;
using Eduprompt.Domain.DTOs.AIHistory;
using Eduprompt.Domain.DTOs.ExpectedOutput;
using Eduprompt.Domain.Entities;

namespace Eduprompt.BLL.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, UserCreateDto>().ReverseMap();
        CreateMap<User, UserUpdateDto>().ReverseMap();
        CreateMap<User, AuthResponseDto>()
            .ForMember(d => d.RoleName, opt => opt.MapFrom(s => s.Role != null ? s.Role.RoleName : null));

        // Role mappings
        CreateMap<Role, RoleDto>().ReverseMap();
        CreateMap<Role, RoleCreateUpdateDto>().ReverseMap();

        // Auth mappings
        CreateMap<GoogleUserInfoDto, User>();
        CreateMap<User, TokenResponseDto>();

        // Cart mappings
        CreateMap<Cart, CartDto>().ReverseMap();
        CreateMap<CartDetail, CartItemDto>().ReverseMap();
        CreateMap<CartDetail, AddCartItemDto>().ReverseMap();
        CreateMap<CartDetail, UpdateCartItemDto>().ReverseMap();

        // Order mappings
        CreateMap<Order, OrderDto>().ReverseMap();
        CreateMap<Order, CreateOrderDto>().ReverseMap();

        // Wishlist mappings
        CreateMap<Wishlist, WishlistDto>().ReverseMap();
        CreateMap<Wishlist, WishlistCreateDto>().ReverseMap();

        // StorageTemplate mappings
        CreateMap<StorageTemplate, StorageTemplateDto>().ReverseMap();
        CreateMap<StorageTemplate, StorageTemplateCreateDto>().ReverseMap();

        // Package & Category mappings
        CreateMap<Package, PackageDto>().ReverseMap();
        CreateMap<Package, CreatePackageDto>().ReverseMap();
        CreateMap<Package, UpdatePackageDto>().ReverseMap();
        CreateMap<PackageCategory, PackageCategoryDto>().ReverseMap();
        CreateMap<PackageCategory, CreatePackageCategoryDto>().ReverseMap();

        // PackageDetail mappings
        CreateMap<PackageDetail, PackageDetailDto>().ReverseMap();
        CreateMap<PackageDetail, CreatePackageDetailDto>().ReverseMap();

        // API Key mappings
        CreateMap<APIKey, APIKeyDto>().ReverseMap();
        CreateMap<APIKey, CreateAPIKeyDto>().ReverseMap();

        // Prompt Instance mappings
        CreateMap<PromptInstance, PromptInstanceDto>().ReverseMap();
        CreateMap<PromptInstance, CreatePromptInstanceDto>().ReverseMap();
        CreateMap<PromptInstance, UpdatePromptInstanceDto>().ReverseMap();

        // Prompt Instance Detail mappings
        CreateMap<PromptInstanceDetail, PromptInstanceDetailDto>().ReverseMap();
        CreateMap<PromptInstanceDetail, CreatePromptInstanceDetailDto>().ReverseMap();

        // Template Architecture mappings
        CreateMap<TemplateArchitecture, TemplateArchitectureDto>().ReverseMap();
        CreateMap<TemplateArchitecture, CreateTemplateArchitectureDto>().ReverseMap();

        // Conversation & Message mappings
        CreateMap<Conversation, ConversationDto>().ReverseMap();
        CreateMap<Conversation, CreateConversationDto>().ReverseMap();
        CreateMap<Message, MessageDto>().ReverseMap();
        CreateMap<Message, CreateMessageDto>().ReverseMap();

        // Payment/Transaction/Wallet mappings
        CreateMap<PaymentMethod, PaymentMethodDto>().ReverseMap();
        CreateMap<PaymentMethod, CreatePaymentMethodDto>().ReverseMap();
        CreateMap<Transaction, TransactionDto>().ReverseMap();
        CreateMap<Transaction, CreateTransactionDto>().ReverseMap();
        CreateMap<Wallet, WalletDto>().ReverseMap();
        CreateMap<Wallet, CreateWalletDto>().ReverseMap();
        CreateMap<Wallet, UpdateWalletDto>().ReverseMap();

        // Post & Feedback mappings
        CreateMap<Post, PostDto>().ReverseMap();
        CreateMap<Post, CreatePostDto>().ReverseMap();
        CreateMap<Feedback, FeedbackDto>().ReverseMap();
        CreateMap<Feedback, CreateFeedbackDto>().ReverseMap();

        // AI History mappings
        CreateMap<AIHistory, AIHistoryDto>().ReverseMap();
        CreateMap<AIHistory, CreateAIHistoryDto>().ReverseMap();

        // Expected Output mappings
        CreateMap<ExpectedOutput, ExpectedOutputDto>().ReverseMap();
        CreateMap<OutputDetail, OutputDetailDto>().ReverseMap();
        CreateMap<ExpectedOutput, CreateExpectedOutputDto>().ReverseMap();
    }
}