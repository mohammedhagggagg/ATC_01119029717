import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from '../../services/event.service';
import { environment } from '../../../environments/environment.development';
import { CommonModule } from '@angular/common';

import Swal from 'sweetalert2';
import { Subscription } from 'rxjs';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-event-details',
  imports: [CommonModule],
  templateUrl: './event-details.component.html',
  styleUrl: './event-details.component.css'
})
export class EventDetailsComponent {
  event: any;
  isAdmin: boolean = false;
  isLogin: boolean = false;
  currentUser: any;
  loading: boolean = true;
  isBooked: boolean = false;
  eventId: number;
  imageBaseUrl: string = environment.baseImageURL;
  selectedPhotoUrl: string | null = null;
  error: string | null = null;
   private authSubscription!: Subscription;
  isZooming: boolean = false; // Track if zoom is active
  zoomX: number = 0; // X position for zoom
  zoomY: number = 0; // Y position for zoom
  constructor(
    private route: ActivatedRoute,
     private eventService: EventService,
      private authService: AuthService,
     private router: Router) {
    this.eventId = +this.route.snapshot.paramMap.get('id')!;
  }

  ngOnInit(): void {
    this.loadEventDetails();
        this.subscribeToAuthChanges();
         if (!this.isLogin) {
    this.isBooked = false;}
  }
   private subscribeToAuthChanges(): void {
    this.authSubscription = this.authService.userData.subscribe({
      next: (data) => {
        if (data) {
          this.isLogin = true;
          this.isAdmin = data.role === 'Admin';
          this.isBooked = this.checkIfBooked(); 
        } else {
          this.isLogin = false;
          this.isAdmin = false;
           this.isBooked = false;
        }
      },
      error: (err) => {
        console.error('Error in auth subscription:', err);
        this.isLogin = false;
        this.isAdmin = false;
        this.isBooked = false;
      }
    });
  }
goBack(): void {
    if (this.isAdmin) {
      this.router.navigate(['/admin-panel']);
    } else {
      this.router.navigate(['/home']);
    }
  }
  goBack02(): void {
    if (this.isAdmin) {
      this.router.navigate(['/home']);
    } 
  }



  loadEventDetails(): void {
  this.loading = true;
  this.eventService.getEventById(this.eventId).subscribe({
    next: (data) => {
      this.event = data;
      this.isBooked = this.checkIfBooked();
      this.loading = false;
    },
    error: (err) => {
      this.error = 'Failed to load event details';
      this.loading = false;
    }
  });
}

bookEvent(eventId: number): void {
   if (!this.isLogin) {
    this.requireLogin();
    return;
  }
  Swal.fire({
    title: 'Confirm Booking',
    text: 'Are you sure you want to book this event?',
    icon: 'question',
    showCancelButton: true,
    confirmButtonColor: '#3085d6',
    cancelButtonColor: '#d33',
    confirmButtonText: 'Yes, book it!'
  }).then((result) => {
    if (result.isConfirmed) {
     
      const bookedEvents = JSON.parse(localStorage.getItem('bookedEvents') || '[]');
      if (!bookedEvents.includes(eventId)) {
        bookedEvents.push(eventId);
        localStorage.setItem('bookedEvents', JSON.stringify(bookedEvents));
      }

      Swal.fire({
        title: 'Booked!',
        text: 'Your event has been booked.',
        icon: 'success',
        timer: 2000,
        timerProgressBar: true
      }).then(() => {
      
      
        this.router.navigate(['/home']); // Navigate to home after booking
      });
    }
  });
}
 
  selectPhoto(url: string) {
    this.selectedPhotoUrl = url;
  }
requireLogin(): void {
  Swal.fire({
    title: 'Login Required',
    text: 'Please login to book this event',
    icon: 'info',
    showCancelButton: true,
    confirmButtonText: 'Login Now',
    cancelButtonText: 'Continue Browsing'
  }).then((result) => {
    if (result.isConfirmed) {
      this.router.navigate(['/login'], {
        queryParams: { returnUrl: this.router.url }
      });
    }
  });
}
checkIfBooked(): boolean {

  if (!this.isLogin) return false;
  
  
  const bookedEvents = localStorage.getItem('bookedEvents');
  if (!bookedEvents) return false;
  
  try {
    const events: number[] = JSON.parse(bookedEvents);
    return events.includes(this.eventId);
  } catch (e) {
    console.error('Error parsing booked events:', e);
    return false;
  }
}
  // Handle right-click to toggle zoom
  onRightClick(event: MouseEvent): void {
    event.preventDefault(); // Prevent default context menu
    this.isZooming = !this.isZooming; // Toggle zoom state

    if (this.isZooming) {
      this.updateZoomPosition(event);
    }
  }

  // Update zoom position based on mouse movement
  onMouseMove(event: MouseEvent): void {
    if (this.isZooming) {
      this.updateZoomPosition(event);
    }
  }

  // Reset zoom when mouse leaves the image
  onMouseLeave(): void {
    this.isZooming = false;
  }

  // Calculate zoom position
  updateZoomPosition(event: MouseEvent): void {
    const img = event.target as HTMLImageElement;
    const rect = img.getBoundingClientRect();
    this.zoomX = ((event.clientX - rect.left) / rect.width) * 100;
    this.zoomY = ((event.clientY - rect.top) / rect.height) * 100;
  }
 ngOnDestroy(): void {
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
  }
}
