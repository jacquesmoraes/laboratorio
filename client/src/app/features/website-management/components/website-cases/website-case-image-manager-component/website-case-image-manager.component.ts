import { Component, Input, Output, EventEmitter, signal, inject, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { WebsiteCaseImage } from '../../../models/website-case.interface';
import { ModalService } from '../../shared/services/modal.service';
import { ImageUploadData } from '../../shared/image-upload-modal/image-upload-modal.component';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-website-case-image-manager',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatDialogModule,
    MatTooltipModule
  ],
  templateUrl: './website-case-image-manager.component.html',
  styleUrls: ['./website-case-image-manager.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class WebsiteCaseImageManagerComponent {
  private modalService = inject(ModalService);

  @Input() images: WebsiteCaseImage[] = [];
  @Input() caseId: number = 0;
  
  @Output() imageAdded = new EventEmitter<WebsiteCaseImage>();
  @Output() imageUpdated = new EventEmitter<{ id: number, image: Partial<WebsiteCaseImage> }>();
  @Output() imageDeleted = new EventEmitter<number>();
  @Output() imageReordered = new EventEmitter<{ fromIndex: number, toIndex: number }>();

  readonly isDragging = signal(false);
  readonly draggedIndex = signal<number | null>(null);

  addImage(): void {
    const uploadData: ImageUploadData = {
      isMainImage: this.images.length === 0
    };

    this.modalService.openImageUploadModal(uploadData).subscribe(result => {
      if (result) {
        // Converter CreateWebsiteCaseImageDto para WebsiteCaseImage
        const newImage: WebsiteCaseImage = {
          id: 0, // Será definido pelo backend
          imageUrl: result.imageUrl,
          altText: result.altText,
          caption: result.caption,
          order: result.order,
          isMainImage: result.isMainImage,
          createdAt: new Date().toISOString()
        };
        this.imageAdded.emit(newImage);
      }
    });
  }

  editImage(image: WebsiteCaseImage): void {
    const uploadData: ImageUploadData = {
      imageUrl: image.imageUrl,
      altText: image.altText,
      caption: image.caption,
      order: image.order,
      isMainImage: image.isMainImage
    };

    this.modalService.openImageUploadModal(uploadData).subscribe(result => {
      if (result) {
        this.imageUpdated.emit({ 
          id: image.id, 
          image: {
            imageUrl: result.imageUrl,
            altText: result.altText,
            caption: result.caption,
            order: result.order,
            isMainImage: result.isMainImage
          }
        });
      }
    });
  }

  deleteImage(image: WebsiteCaseImage): void {
    Swal.fire({
      title: 'Confirmar exclusão',
      text: `Tem certeza que deseja excluir esta imagem?`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Sim, excluir',
      cancelButtonText: 'Cancelar'
    }).then((result) => {
      if (result.isConfirmed) {
        this.imageDeleted.emit(image.id);
      }
    });
  }

  setMainImage(image: WebsiteCaseImage): void {
    if (!image.isMainImage) {
      this.imageUpdated.emit({ 
        id: image.id, 
        image: { isMainImage: true } 
      });
    }
  }

  // Drag and Drop functionality
  onDragStart(event: DragEvent, index: number): void {
    this.isDragging.set(true);
    this.draggedIndex.set(index);
    if (event.dataTransfer) {
      event.dataTransfer.effectAllowed = 'move';
    }
  }

  onDragOver(event: DragEvent, index: number): void {
    event.preventDefault();
    if (event.dataTransfer) {
      event.dataTransfer.dropEffect = 'move';
    }
  }

  onDrop(event: DragEvent, dropIndex: number): void {
    event.preventDefault();
    const dragIndex = this.draggedIndex();
    
    if (dragIndex !== null && dragIndex !== dropIndex) {
      this.imageReordered.emit({ fromIndex: dragIndex, toIndex: dropIndex });
    }
    
    this.isDragging.set(false);
    this.draggedIndex.set(null);
  }

  onDragEnd(): void {
    this.isDragging.set(false);
    this.draggedIndex.set(null);
  }

  getImagePreviewUrl(imageUrl: string): string {
    return imageUrl;
  }

  trackByImageId(index: number, image: WebsiteCaseImage): number {
    return image.id;
  }
  openReorderModal(): void {
    if (this.images.length < 2) {
      console.warn('É necessário pelo menos 2 imagens para reordenar.');
      return;
    }

    this.modalService.openReorderImagesModal([...this.images]).subscribe(result => {
      if (result) {
        // Emitir evento de reordenação para cada mudança
        result.forEach((image, newIndex) => {
          const oldIndex = this.images.findIndex(img => img.id === image.id);
          if (oldIndex !== newIndex) {
            this.imageReordered.emit({ 
              fromIndex: oldIndex, 
              toIndex: newIndex 
            });
          }
        });
      }
    });
  }

}