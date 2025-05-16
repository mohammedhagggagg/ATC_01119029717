import { Photo } from './../../interfaces/photo';
import { Component, OnInit } from '@angular/core';
import { EventService } from '../../services/event.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CategoryService } from '../../services/category.service';
import { environment } from '../../../environments/environment';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-admin-panel',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css'],
})
export class AdminPanelComponent implements OnInit {
  events: any[] = [];
  categories: any[] = [];
  pageSize: number = 6;
  pageIndex: number = 1;
  totalPages: number = 0;
  showForm: boolean = false;
  isEditMode: boolean = false;
  selectedEvent: any = null;
  EmailAdmin: string = 'admin@gmail.com';
  imageUrl: string = 'assets/img/Picsart_25-01-29_12-50-29-526.jpg';
  baseImageUrl: string = environment.baseImageURL;
  formError: string = '';
  activeTab: string = 'events';
  // Form Fields
  eventForm: any = {
    title: '',
    description: '',
    date: '',
    venue: '',
    price: 0,
    categoryId: 0,
  };

  existingPhotos: Photo[] = [];
  photosToAdd: File[] = [];
  photoIdsToDelete: number[] = [];

  constructor(
    private _AuthService : AuthService,
     private router: Router,
    private eventService: EventService,
    private categoryService: CategoryService,
     private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.loadEvents();
    this.loadCategories();
  }

  loadEvents(): void {
    this.eventService.getAllEvents(this.pageSize, this.pageIndex).subscribe({
      next: (response) => {
        console.log('Events Response:', response);
        this.events = response.events;
        this.totalPages = response.totalPages;
      },
      error: (error) => {
        console.error('Error loading events:', error);
      },
    });
  }

  loadCategories(): void {
    this.categoryService.getAllCategories().subscribe({
      next: (response) => {
        console.log('Categories Response:', response);
        this.categories = response;
        if (this.categories.length > 0) {
          this.eventForm.categoryId = this.categories[0].id;
        }
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      },
    });
  }

  showAddForm(): void {
    this.showForm = true;
    this.isEditMode = false;
    this.resetForm();
    this.formError = '';
  }


  showEditForm(event: any): void {
    this.showForm = true;
    this.isEditMode = true;
    this.selectedEvent = event;

    this.eventForm = {
      title: event.title,
      description: event.description,
      date: new Date(event.date).toISOString().split('T')[0],
      venue: event.venue,
      price: event.price,
      categoryId: event.category.id,
    };

    this.existingPhotos = event.photos || [];
    this.photosToAdd = [];
    this.photoIdsToDelete = [];
    this.formError = '';
  }

  resetForm(): void {
    this.eventForm = {
      title: '',
      description: '',
      date: '',
      venue: '',
      price: 0,
      categoryId: this.categories.length > 0 ? this.categories[0].id : 0,
      photos: [],
    };
    this.photosToAdd = [];
    this.photoIdsToDelete = [];
    this.selectedEvent = null;
    this.formError = '';
  }
getPreviewUrl(file: File): string {
  return URL.createObjectURL(file);
}

onFileChange(event: any): void {
  if (event.target.files && event.target.files.length > 0) {
    const newFiles = Array.from(event.target.files) as File[];
    this.photosToAdd = [...this.photosToAdd, ...newFiles];
    event.target.value = ''; // Reset input to allow selecting same files again
  }
}
  validateForm(): boolean {
    if (!this.eventForm.title || this.eventForm.title.trim() === '') {
      this.formError = 'Title is required';
      return false;
    }
    if (
      !this.eventForm.description ||
      this.eventForm.description.trim() === ''
    ) {
      this.formError = 'Description is required';
      return false;
    }
    if (!this.eventForm.date) {
      this.formError = 'Date is required';
      return false;
    }
    if (!this.eventForm.venue || this.eventForm.venue.trim() === '') {
      this.formError = 'Venue is required';
      return false;
    }
    if (!this.eventForm.price || this.eventForm.price <= 0) {
      this.formError = 'Price must be greater than 0';
      return false;
    }
    if (!this.eventForm.categoryId || this.eventForm.categoryId <= 0) {
      this.formError = 'Please select a category';
      return false;
    }
    this.formError = '';
    return true;
  }
  createEvent(): void {
    if (!this.validateForm()) return;

    this.eventService
      .createEvent(this.eventForm, this.photosToAdd)
      .subscribe({
        next: (response) => {
          console.log('Event Created:', response);
          this.loadEvents();
          this.showForm = false;
          this.resetForm();
          document.getElementById('eventModal')?.classList.remove('show');
          document.body.classList.remove('modal-open');
          const modalBackdrop =
            document.getElementsByClassName('modal-backdrop')[0];
          if (modalBackdrop) modalBackdrop.remove();
          this.toastr.success('Event Created Successfully')
        },
        error: (error) => {
          console.error('Error creating event:', error);
          this.formError = 'Error creating event. Please try again.';
           this.toastr.error('Error creating event')
        },
      });
  }


  updateEvent(): void {
  if (!this.validateForm() || !this.selectedEvent) return;

  this.eventService.updateEvent(
    this.selectedEvent.id,
    this.eventForm,
    this.photosToAdd || [],
    this.photoIdsToDelete || [] 
  ).subscribe({
    next: (response) => {
      console.log('Event Updated:', response);
      this.toastr.success('Event updated successfully!');
      this.loadEvents();
      this.closeModal();
    },
    error: (error) => {
      console.error('Error updating event:', error);
      this.toastr.error('Error updating event. Please try again.');
      this.formError = 'Error updating event. Please try again.';
    }
  });
}


closeModal(): void {
  this.showForm = false;
  this.resetForm();
  document.getElementById('eventModal')?.classList.remove('show');
  document.body.classList.remove('modal-open');
  const modalBackdrop = document.getElementsByClassName('modal-backdrop')[0];
  if (modalBackdrop) modalBackdrop.remove();
}
markPhotoForDeletion(photoId: number): void {
  if (!this.photoIdsToDelete.includes(photoId)) {
    this.photoIdsToDelete.push(photoId);
    this.existingPhotos = this.existingPhotos.filter(p => p.id !== photoId);
    this.toastr.info('Photo marked for deletion', '', { timeOut: 1500 });
  }
}
removeNewPhoto(index: number): void {
  this.photosToAdd.splice(index, 1);
}


deleteEvent(id: number): void {
  Swal.fire({
    title: 'Are you sure?',
    text: "You won't be able to revert this!",
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#d33',
    cancelButtonColor: '#3085d6',
    confirmButtonText: 'Yes, delete it!',
    cancelButtonText: 'Cancel'
  }).then((result) => {
    if (result.isConfirmed) {
      this.eventService.deleteEvent(id).subscribe({
        next: () => {
          Swal.fire({
            icon: 'success',
            title: 'Deleted!',
            text: 'The event has been deleted.',
            timer: 1500,
            showConfirmButton: false
          });
          this.loadEvents();
        },
        error: (error) => {
          Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'An error occurred while deleting the event.',
          });
          console.error('Error deleting event:', error);
        },
      });
    }
  });
}

  
  viewDetails(eventId: number): void {
    this.router.navigate(['/events', eventId]);
  }
  changePage(newPage: number): void {
    if (newPage >= 1 && newPage <= this.totalPages) {
      this.pageIndex = newPage;
      this.loadEvents();
    }
  }
  isFormValid(): boolean {
  return this.eventForm.title && 
         this.eventForm.description && 
         this.eventForm.date && 
         this.eventForm.venue && 
         this.eventForm.price > 0 && 
         this.eventForm.categoryId;
}

  setActiveTab(tab: string): void {
    this.activeTab = tab;
    if (tab === 'event') this.loadEvents();
  }

    logout(): void {
    Swal.fire({
      title: 'Are you sure?',
      text: 'You will be logged out of your account.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#6c7fd8',
      cancelButtonColor: '#dc3545',
      confirmButtonText: 'Yes, logout',
      cancelButtonText: 'Cancel'
    }).then((result) => {
      if (result.isConfirmed) {
          this._AuthService.signout();
        // this.router.navigate(['/login']);
        Swal.fire({
          title: 'Logged out!',
          text: 'You have been successfully logged out.',
          icon: 'success',
          confirmButtonColor: '#6c7fd8',
          timer: 1500,
          showConfirmButton: false
        });
      }
    });
  }
 getUpcomingEvents() {
    const today = new Date();
    return this.events.filter(event => new Date(event.date) > today);
  }
  ngOnDestroy() {
  this.photosToAdd.forEach(file => {
    URL.revokeObjectURL(this.getPreviewUrl(file));
  });
}
}
