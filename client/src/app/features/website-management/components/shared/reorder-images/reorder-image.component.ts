import { Component, input, output, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';

import { WebsiteCaseImage } from '../../../models/website-case.interface';

@Component({
  selector: 'app-reorder-images',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule],
  template: `
    <div class="reorder-images-modal">
      <!-- Header -->
      <div class="modal-header">
        <h2 class="text-xl font-semibold text-gray-900">
          Reordenar Imagens
        </h2>
        <button 
          mat-icon-button 
          (click)="close()"
          class="text-gray-500 hover:text-gray-700">
          <mat-icon>close</mat-icon>
        </button>
      </div>

      <!-- Content -->
      <div class="modal-content">
        <p class="text-gray-600 mb-4">
          Clique nas imagens para reordená-las. A primeira imagem será a principal.
        </p>

        <div class="images-grid">
          @for (image of currentImages(); track image.id; let i = $index) {
            <div 
              class="image-item"
              [class.main-image]="i === 0"
              [class.selected]="i === selectedIndex()"
              (click)="selectImage(i)">
              <img 
                [src]="image.imageUrl" 
                [alt]="image.altText"
                class="w-full h-32 object-cover rounded-lg">
              <div class="image-info">
                <p class="text-sm font-medium text-gray-900">{{ image.caption }}</p>
                <p class="text-xs text-gray-500">Ordem: {{ i + 1 }}</p>
                @if (i === 0) {
                  <span class="text-xs bg-blue-100 text-blue-800 px-2 py-1 rounded">
                    Principal
                  </span>
                }
              </div>
            </div>
          }
        </div>
      </div>

      <!-- Actions -->
      <div class="modal-actions">
        <button 
          type="button"
          mat-button 
          (click)="close()"
          class="text-gray-600 hover:text-gray-900">
          Cancelar
        </button>
        <button 
          type="button"
          mat-raised-button 
          color="primary"
          (click)="saveOrder()"
          class="bg-[#276678] hover:bg-[#334a52]">
          <mat-icon>save</mat-icon>
          Salvar Ordem
        </button>
      </div>
    </div>
  `,
  styles: [`
    .reorder-images-modal {
      width: 100%;
      max-width: 800px;
      margin: 0 auto;
    }

    .modal-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1.5rem 1.5rem 0;
      border-bottom: 1px solid #e5e7eb;
      margin-bottom: 1.5rem;
    }

    .modal-content {
      padding: 0 1.5rem;
      max-height: 70vh;
      overflow-y: auto;
    }

    .modal-actions {
      display: flex;
      justify-content: flex-end;
      gap: 1rem;
      padding: 1.5rem;
      border-top: 1px solid #e5e7eb;
      margin-top: 1.5rem;
    }

    .images-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
      gap: 1rem;
    }

    .image-item {
      border: 2px solid #e5e7eb;
      border-radius: 8px;
      padding: 0.5rem;
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover {
        border-color: #276678;
        transform: translateY(-2px);
      }

      &.main-image {
        border-color: #276678;
        background-color: #f0f9ff;
      }

      &.selected {
        border-color: #a288a9;
        background-color: #fdf2f8;
        transform: scale(1.02);
      }
    }

    .image-info {
      margin-top: 0.5rem;
      text-align: center;
    }
  `]
})
export class ReorderImagesComponent {
  private readonly dialogRef = inject(MatDialogRef<ReorderImagesComponent>);
  private readonly snackBar = inject(MatSnackBar);
  private readonly data = inject<{ images: WebsiteCaseImage[] }>(MAT_DIALOG_DATA);

  // Signal para armazenar as imagens atuais
  currentImages = signal<WebsiteCaseImage[]>([]);
  selectedIndex = signal<number | null>(null);

  constructor() {
    // Inicializar com as imagens recebidas
    this.currentImages.set(this.data.images);
  }

  selectImage(index: number): void {
    this.selectedIndex.set(index);
  }

  saveOrder(): void {
    const reordered = [...this.currentImages()];
    const selected = this.selectedIndex();
    
    if (selected !== null && selected !== 0) {
      // Move selected image to first position
      const [selectedImage] = reordered.splice(selected, 1);
      reordered.unshift(selectedImage);
      
      // Update order numbers
      reordered.forEach((image, index) => {
        image.order = index + 1;
        image.isMainImage = index === 0;
      });
    }

    this.dialogRef.close(reordered);
    this.snackBar.open('Ordem das imagens salva com sucesso!', 'Fechar', { duration: 3000 });
  }

  close(): void {
    this.dialogRef.close();
  }
}