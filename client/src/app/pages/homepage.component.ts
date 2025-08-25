import { Component, ChangeDetectionStrategy, OnInit, OnDestroy, signal, inject, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { WebsiteWorkTypeService } from '../features/website-management/services/website-worktype.service';
import { WebsiteCaseService } from '../features/website-management/services/website-case.service';
import { WebsiteWorkType } from '../features/website-management/models/website-worktype.interface';
import { WebsiteCase } from '../features/website-management/models/website-case.interface';

@Component({
  selector: 'app-homepage',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './homepage.component.html',
  styleUrls: ['./homepage.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  encapsulation: ViewEncapsulation.None
})
export class HomepageComponent implements OnInit, OnDestroy {
  private router = inject(Router);
  private websiteCaseService = inject(WebsiteCaseService);
  private websiteWorkTypeService = inject(WebsiteWorkTypeService);
  readonly isDarkMode = signal(false);
  readonly mobileMenuOpen = signal(false);
  readonly currentSlide = signal(0);
  readonly showScrollTop = signal(false);
  readonly websiteCases = signal<WebsiteCase[]>([]);
  readonly websiteWorkTypes = signal<WebsiteWorkType[]>([]);
  readonly slides = [
    { src: 'assets/images/imagecover.jpg', alt: 'Modelo 3D de prótese' },
    { src: 'assets/images/imagecover2.jpg', alt: 'Modelo 3D de prótese' }
  ];

  private carouselInterval?: number;
  private scrollListener?: () => void;

  ngOnInit(): void {
    this.initTheme();
    this.initCarousel();
    this.initScrollListener();
    this.initSmoothScroll();
    this.loadHomepageData();
  }

  ngOnDestroy(): void {
    if (this.carouselInterval) {
      clearInterval(this.carouselInterval);
    }
    if (this.scrollListener) {
      window.removeEventListener('scroll', this.scrollListener);
    }
  }

  private loadHomepageData(): void {
    
    this.websiteCaseService.getActiveForHomepage().subscribe(cases => {
      this.websiteCases.set(cases);
    });
     this.websiteWorkTypeService.getActive().subscribe(workTypes => {
      this.websiteWorkTypes.set(workTypes);
    });
  }


  viewCaseDetails(caseId: number): void {
    this.router.navigate(['/case', caseId]);
  }

  private initTheme(): void {
    const savedTheme = localStorage.getItem('homepage-theme');
    const systemPrefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    
    if (savedTheme === 'dark' || (!savedTheme && systemPrefersDark)) {
      this.isDarkMode.set(true);
      document.documentElement.classList.add('dark');
    }
  }

  


  private initCarousel(): void {
    this.carouselInterval = setInterval(() => {
      this.nextSlide();
    }, 5000);
  }

  private initScrollListener(): void {
    this.scrollListener = () => {
      this.showScrollTop.set(window.pageYOffset > 300);
    };
    window.addEventListener('scroll', this.scrollListener);
  }

  private initSmoothScroll(): void {
    // Angular will handle this with smooth scrolling behavior
  }

  toggleTheme(): void {
    const newTheme = !this.isDarkMode();
    this.isDarkMode.set(newTheme);
    
    if (newTheme) {
      document.documentElement.classList.add('dark');
      localStorage.setItem('homepage-theme', 'dark');
    } else {
      document.documentElement.classList.remove('dark');
      localStorage.setItem('homepage-theme', 'light');
    }
  }

  toggleMobileMenu(): void {
    this.mobileMenuOpen.update(open => !open);
  }

  closeMobileMenu(): void {
    this.mobileMenuOpen.set(false);
  }

  nextSlide(): void {
    this.currentSlide.update(current => 
      current === this.slides.length - 1 ? 0 : current + 1
    );
  }

  prevSlide(): void {
    this.currentSlide.update(current => 
      current === 0 ? this.slides.length - 1 : current - 1
    );
  }

  goToSlide(index: number): void {
    this.currentSlide.set(index);
    // Reset carousel interval
    if (this.carouselInterval) {
      clearInterval(this.carouselInterval);
      this.initCarousel();
    }
  }
  getLogoPath(): string {
    return this.isDarkMode() ? 'assets/images/logo-white.png' : 'assets/images/logo.png';
  }
scrollToSection(sectionId: string): void {
  const element = document.getElementById(sectionId);
  if (element) {
    element.scrollIntoView({ behavior: 'smooth' });
 
  }
}
  scrollToTop(): void {
    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  }

  goToLogin(): void {
    window.open('/login', '_blank');
  }
}