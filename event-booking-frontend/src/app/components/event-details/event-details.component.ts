import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from '../../services/event.service';
import { environment } from '../../../environments/environment.development';
import { CommonModule } from '@angular/common';

import Swal from 'sweetalert2';

@Component({
  selector: 'app-event-details',
  imports: [CommonModule],
  templateUrl: './event-details.component.html',
  styleUrl: './event-details.component.css'
})
export class EventDetailsComponent {
  event: any;
  loading: boolean = true;
  isBooked: boolean = false;
  eventId: number;
  imageBaseUrl: string = environment.baseImageURL;
  selectedPhotoUrl: string | null = null;
  error: string | null = null;
  isZooming: boolean = false; // Track if zoom is active
  zoomX: number = 0; // X position for zoom
  zoomY: number = 0; // Y position for zoom
  constructor(
    private route: ActivatedRoute,
     private eventService: EventService,
     private router: Router) {
    this.eventId = +this.route.snapshot.paramMap.get('id')!;
  }

  ngOnInit(): void {
    this.loadEventDetails();
  }

  loadEventDetails(): void {
    this.loading = true;
    this.eventService.getEventById(this.eventId).subscribe({
      next: (data) => {
        console.log('API Response:', data);
        this.event = data;
        this.isBooked = this.checkIfBooked();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading event details:', err);
        this.error = 'Failed to load event details. Check console for details.';
        this.loading = false;
      }
    });
  }

  bookEvent(): void {
  Swal.fire({
      title: 'Success!',
      text: 'You have successfully booked the event!',
      icon: 'success',
      confirmButtonText: 'OK',
      confirmButtonColor: '#3085d6',
      timer: 2000, // Auto-close after 2 seconds
      timerProgressBar: true
    }).then((result) => {
      if (result.isConfirmed || result.dismiss === Swal.DismissReason.timer) {
        this.isBooked = true; // Update booked state
        this.router.navigate(['/home']); // Navigate to home page
      }
    });
   
  }
 
  selectPhoto(url: string) {
    this.selectedPhotoUrl = url;
  }
  checkIfBooked(): boolean {
    
    return false; 
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
}
