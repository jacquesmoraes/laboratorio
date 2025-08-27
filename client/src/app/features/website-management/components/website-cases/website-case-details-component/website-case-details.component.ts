import { Component, OnInit, signal, inject, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { WebsiteCase } from '../../../models/website-case.interface';
import { WebsiteCaseService } from '../../../services/website-case.service';

@Component({
  selector: 'app-case-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './website-case-details.component.html',
  styleUrls: ['./website-case-details.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class WebsiteCaseDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private websiteCaseService = inject(WebsiteCaseService);

  readonly caseDetails = signal<WebsiteCase | null>(null);
  readonly isLoading = signal(true);
  readonly currentImageIndex = signal(0);
  readonly isAdminContext = signal(false);

  ngOnInit(): void {
    // Verificar se está no contexto admin baseado na URL atual
    const currentUrl = this.router.url;
    this.isAdminContext.set(currentUrl.includes('/admin/'));
    this.loadCaseDetails();
  }

  private loadCaseDetails(): void {
    const caseId = this.route.snapshot.paramMap.get('id');
    
    if (!caseId) {
      this.router.navigate(['/']);
      return;
    }

    this.websiteCaseService.getDetailsById(+caseId).subscribe({
      next: (caseData) => {
        this.caseDetails.set(caseData);
        this.isLoading.set(false);
      },
      error: () => {
        this.router.navigate(['/']);
      }
    });
  }

  nextImage(): void {
    const caseData = this.caseDetails();
    if (caseData && caseData.images.length > 0) {
      this.currentImageIndex.update(current => 
        current === caseData.images.length - 1 ? 0 : current + 1
      );
    }
  }

  prevImage(): void {
    const caseData = this.caseDetails();
    if (caseData && caseData.images.length > 0) {
      this.currentImageIndex.update(current => 
        current === 0 ? caseData.images.length - 1 : current - 1
      );
    }
  }

  goToImage(index: number): void {
    this.currentImageIndex.set(index);
  }

  goBack(): void {
    if (this.isAdminContext()) {
      this.router.navigate(['/admin/website-management/cases']);
    } else {
      this.router.navigate(['/']);
    }
  }
  
  onImageError(event: any): void {
    event.target.src = 'assets/images/placeholder-image.jpg';
  }
}