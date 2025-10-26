# 🐾 PuppyLove - Dog Training & Breeding Platform

A comprehensive web application connecting dog trainers, breeders, and pet owners in South Africa. Built with ASP.NET Core MVC and MySQL.

## 🌟 Overview

PuppyLove is a platform that facilitates connections between dog professionals and pet owners. Trainers can showcase their skills and manage training programs, breeders can list puppies and manage litters, while
clients can find services and adopt puppies.

## 👥 User Roles & Features

## 🏠 Clients

Browse & Adopt: View available puppies from registered breeders

Find Professionals: Search and review trainers and breeders

Training Requests: Submit training requests to trainers

Profile Management: Personal profiles with contact information

Review System: Rate and review service providers

## 🎓 Trainers

Skill Management: Showcase training specialties and skill levels

Training Programs: Manage ongoing training sessions and progress tracking

Park Recommendations: Share recommended dog parks with the community

Request Management: Accept/reject training requests from clients

Profile Customization: Professional profiles with ratings and reviews

## 🐕 Breeders

Litter Management: Create and manage puppy litters

Puppy Listings: Register individual puppies with detailed information

Request System: Handle adoption requests from clients

Breed Specialization: Showcase specific breed expertise

Kennel Management: Professional breeder profiles with licensing

## 🛠️ Technology Stack

### Backend
ASP.NET Core 8.0 MVC

Entity Framework Core

MySQL Database

### Frontend
Razor Views with Bootstrap

jQuery and JavaScript

CSS3 with responsive design

### Key Features
Role-based authentication (Trainer, Breeder, Client)

File upload for images

Review and rating system

Real-time request management

Responsive web design

# 🚀 Getting Started

## Prerequisites
.NET 8.0 SDK

MySQL Server 8.0+

Visual Studio 2022 or VS Code

## Installation
**Clone the Repository**
```bash
git clone https://github.com/your-username/PuppyLove.git
```

**Navigate into the directory**
```bash
cd PuppyLove
```

## Directory structure

```markdown
PuppyLove/
│
├── Program.cs # Entry point of the application; configures services, roles, cookies, and middleware
│
├── Properties/
│ └── launchSettings.json # Development launch configuration for IIS Express and Kestrel
│
├── Data/
│ └── ApplicationDbContext.cs # Entity Framework Core context; defines database sets and relationships
│
├── Migrations/ # Entity Framework migration files for schema changes
│
├── Controllers/ # MVC controllers — handle user interactions and business logic
│ ├── AccountController.cs # Manages user registration, login, logout, and role assignment
│ ├── BreederDashboardController.cs # Handles breeder profile management and dashboard updates
│ ├── ClientDashboardController.cs # Displays client information and manages profile updates
│ ├── DashboardController.cs # Trainer dashboard — manages skills, progress, and training requests
│ ├── HomeController.cs # Controls home page, privacy page, and error handling
│ ├── LitterController.cs # Allows breeders to create, view, and delete litters
│ ├── ParkRecommendationController.cs # Trainers can recommend parks and upload related images
│ ├── PuppyController.cs # Breeders manage puppies (registration, sale marking, and details)
│ ├── PuppyRequestController.cs # Handles client puppy adoption requests and breeder responses
│ ├── ReviewController.cs # Enables clients to write and view reviews for breeders/trainers
│ ├── SkillController.cs # Handles skill management and editing for trainers
│ ├── TrainingProgressController.cs # Logs and updates progress of dog training sessions
│ └── TrainingRequestController.cs # Handles training request creation and approval workflow
│
├── Models/ # Entity, ViewModel, and domain model definitions
│ ├── ApplicationDbContextModel.cs # EF-generated model snapshot of the database
│ ├── ApplicationUser.cs # Extends IdentityUser with role-specific fields (TrainerId, ClientId, etc.)
│
│ ├── Breeder.cs # Entity representing a breeder in the system
│ ├── BreederDashboardViewModel.cs # View model for displaying breeder dashboard info
│ ├── BreederRegisterViewModel.cs # Captures breeder registration data
│ ├── BreederSeeder.cs # Seeds initial breeder or breed-related data
│
│ ├── BreedSpecialization.cs # Defines specialization or expertise of breeders
│ ├── Breedtype.cs # Defines available dog breeds
│
│ ├── Client.cs # Entity representing a client user
│ ├── ClientDashboardViewModel.cs # View model for client dashboard details
│ ├── ClientRegisterViewModel.cs # Captures client registration data
│
│ ├── CreateLitterViewModel.cs # Used for creating a new litter entry
│ ├── DashboardViewModel.cs # Trainer dashboard data aggregation
│
│ ├── ErrorViewModel.cs # Used for error page rendering
│ ├── HomePageViewModel.cs # Provides data for home page (top-rated users, parks, etc.)
│
│ ├── Litter.cs # Entity representing a breeder’s litter
│
│ ├── LoginViewModel.cs # Captures user login credentials
│
│ ├── Parkrecommendation.cs # Entity for trainer-recommended parks
│
│ ├── Puppy.cs # Entity representing an individual puppy
│ ├── PuppyDetails.cs # Extended puppy view with detailed attributes
│ ├── Puppyrequest.cs # Entity linking clients to requested puppies
│ ├── PuppyRequestViewModel.cs # View model for adoption request display and input
│ ├── RegisterPuppyViewModel.cs # Used when breeders register new puppies
│
│ ├── Review.cs # Entity for reviews left by clients
│ ├── ReviewViewModel.cs # Used for displaying and submitting reviews
│
│ ├── Skill.cs # Entity defining a trainer skill
│ ├── SkillSelectionViewModel.cs # Used in skill selection UI for trainers
│
│ ├── Trainer.cs # Entity representing a dog trainer
│ ├── TrainerRegisterViewModel.cs # Captures trainer registration data
│ ├── Trainerskill.cs # Many-to-many relation between trainers and skills
│ ├── TrainerSkillDisplayViewModel.cs # Displays trainer skills and proficiency levels
│
│ ├── TrainingProgress.cs # Logs and tracks progress for training sessions
│ ├── Trainingrequest.cs # Entity for client training requests
│ └── TrainingRequestViewModel.cs # View model for training request display and interaction
│
├── Views/ # Razor view templates for UI rendering
│ ├── Account/
│ │ ├── ChooseRole.cshtml # Page for selecting a role during registration
│ │ ├── Login.cshtml # Login form
│ │ ├── RegisterAsBreeder.cshtml # Breeder registration form
│ │ ├── RegisterAsClient.cshtml # Client registration form
│ │ └── RegisterAsTrainer.cshtml # Trainer registration form
│
│ ├── BreederDashboard/
│ │ └── Index.cshtml # Breeder dashboard main page
│
│ ├── ClientDashboard/
│ │ ├── Breeder.cshtml # Lists breeders available for clients
│ │ ├── BreederReviews.cshtml # Displays reviews about breeders
│ │ ├── Index.cshtml # Client dashboard home
│ │ ├── Puppies.cshtml # Displays puppies available to clients
│ │ ├── Puppy.cshtml # Shows detailed info on a single puppy
│ │ ├── Trainer.cshtml # Lists trainers available for clients
│ │ └── TrainerReview.cshtml # Displays reviews about trainers
│
│ ├── Dashboard/
│ │ ├── EditSkills.cshtml # Trainer page for selecting and editing skills
│ │ └── TrainingRequests.cshtml # Trainer page for managing incoming training requests
│
│ ├── Home/
│ │ ├── Index.cshtml # Home page displaying top-rated trainers and breeders
│ │ └── Privacy.cshtml # Privacy policy page
│
│ ├── Litter/
│ │ ├── Index.cshtml # List of litters owned by a breeder
│ │ └── Details.cshtml # Detailed litter view showing puppies
│
│ ├── ParkRecommendation/
│ │ └── Create.cshtml # Form for trainers to create park recommendations
│
│ ├── Puppy/
│ │ ├── Index.cshtml # List of registered puppies
│ │ ├── Details.cshtml # Detailed puppy info
│ │ └── Register.cshtml # Form for registering a new puppy
│
│ ├── PuppyRequest/
│ │ ├── _RequestsList.cshtml # Partial view for listing requests
│ │ ├── BrowsePuppies.cshtml # Clients browse available puppies
│ │ ├── IncomingRequests.cshtml # Breeder view of client adoption requests
│ │ ├── MyRequests.cshtml # Client view of their own adoption requests
│ │ ├── RequestDetails.cshtml # Details of a specific adoption request
│ │ └── RequestPuppy.cshtml # Form for clients to request a puppy
│
│ ├── Review/
│ │ ├── BrowseBreeders.cshtml # Browse breeder profiles and reviews
│ │ ├── BrowseTrainers.cshtml # Browse trainer profiles and reviews
│ │ └── CreateReview.cshtml # Form for creating a new review
│
│ ├── Shared/
│ │ ├── _Layout.cshtml # Global layout for all pages (header, footer, nav)
│ │ ├── _PuppyCard.cshtml # Reusable card component for displaying puppy info
│ │ ├── _TopRatedSliders.cshtml # Component showing top-rated breeders/trainers
│ │ └── _ValidationScriptsPartial.cshtml # Built-in validation script partial for forms
│
│ ├── Skills/
│ │ └── SkillSelection.cshtml # UI for trainers to select their skills
│
│ ├── _ViewImports.cshtml # Imports namespaces for all views
│ └── _ViewStart.cshtml # Sets default layout for all views
│
└── wwwroot/ # Static web assets (CSS, JS, uploaded images, etc.)
├── images/
│ ├── profiles/ # User profile pictures
│ ├── puppies/ # Puppy photos uploaded by breeders
│ └── default-profile.jpg # Default profile image
├── uploads/
│ └── parks/ # Park recommendation images uploaded by trainers
├── css/ # Custom and bootstrap CSS
├── js/ # Client-side scripts
└── lib/ # Third-party libraries (Bootstrap, jQuery, etc.)
```

📄 License

This project is licensed under the MIT License - see the LICENSE.md file for details.

📞 Support

For support, email support@puppylove.com or create an issue in the GitHub repository.

Built with ❤️ for the dog-loving community in South Africa.

All rights reserved © 2025 Ibiza Development Team.
