import { Component, ElementRef, OnInit, Renderer2, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EventService } from '../../services/event.service';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Observable, Subscription } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CarouselModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  events: any[] = [];
  filterForm: FormGroup;
  pageSize: number = 6;
  pageIndex: number = 1;
  totalItems: number = 0;
  imageBaseUrl: string = environment.baseImageURL;
  showCarousel: { [key: number]: boolean } = {};
  categorySlides: any[] = [];
  isLoading: boolean = true;
  private eventsSubscription: Subscription | undefined;

  constructor(
    private router: Router,
    private eventService: EventService,
    private fb: FormBuilder,
    private renderer: Renderer2,
    private el: ElementRef
  ) {
    this.filterForm = this.fb.group({
      categoryId: [null],
      minPrice: [null],
      maxPrice: [null],
      startDate: [null],
      endDate: [null]
    });
  }

  ngOnInit(): void {
    this.loadEvents();
    this.categorySlides = [
      [{ id: 1, name: 'Music', photo: 'music.png' }, { id: 2, name: 'Sports', photo: 'sports.png' }],
      [{ id: 3, name: 'Conferences', photo: 'conference.png' }]
    ];
  }

  ngAfterViewInit(): void {
    const zoomContainers = this.el.nativeElement.querySelectorAll('.zoom-container');
    zoomContainers.forEach((container: HTMLElement) => {
      const img = container.querySelector('.zoom-on-hover') as HTMLImageElement;
      this.renderer.listen(container, 'mousemove', (e: MouseEvent) => {
        const rect = container.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;
        const centerX = rect.width / 2;
        const centerY = rect.height / 2;
        const moveX = (x - centerX) / 20;
        const moveY = (y - centerY) / 20;
        img.style.transformOrigin = `${x}px ${y}px`;
        img.style.transform = `scale(1.1) translate(${moveX}px, ${moveY}px)`;
      });
      this.renderer.listen(container, 'mouseleave', () => {
        img.style.transform = 'scale(1)';
      });
    });
  }

  loadEvents(): void {
    this.isLoading = true; 
    const { categoryId, minPrice, maxPrice, startDate, endDate } = this.filterForm.value;
    this.eventService.getAllEvents(
      this.pageSize,
      this.pageIndex,
      categoryId || null,
      maxPrice || null,
      minPrice || null,
      startDate || null,
      endDate || null
    ).pipe(
      finalize(() => this.isLoading = false) 
    ).subscribe({
      next: (response: any) => {
        this.events = response.events || [];
        this.totalItems = response.totalCount || 0;
      },
      error: (error: any) => {
        console.error('Error loading events:', error);
      }
    });
  }

  get totalPages(): number[] {
    const pagesCount = Math.ceil(this.totalItems / this.pageSize);
    return Array.from({ length: pagesCount }, (_, i) => i + 1);
  }
  get pageGroups(): number[][] {
    const groupSize = 4; 
    const groups: number[][] = [];
    for (let i = 0; i < this.totalPages.length; i += groupSize) {
      groups.push(this.totalPages.slice(i, i + groupSize));
    }
    return groups;
  }
  onFilter(): void {
    this.pageIndex = 1;
    this.loadEvents();
  }

  filterByCategory(categoryId: number): void {
    this.filterForm.patchValue({ categoryId });
    this.onFilter();
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages.length && page !== this.pageIndex) {
      this.pageIndex = page;
      this.loadEvents();
    }
  }

  isBooked(eventId: number): boolean {
    return false;
  }

  bookEvent(eventId: number): void {
    console.log('Booking event:', eventId);
  }

  viewDetails(eventId: number): void {
    this.router.navigate(['/events', eventId]);
  }

  ngOnDestroy(): void {
    if (this.eventsSubscription) {
      this.eventsSubscription.unsubscribe();
    }
  }
}