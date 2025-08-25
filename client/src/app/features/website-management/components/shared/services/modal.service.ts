import { Injectable, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { HttpClient, HttpEvent, HttpEventType } from '@angular/common/http';
import { Observable, map, catchError, throwError } from 'rxjs';
import { environment } from '../../../../../../environments/environment';
import { ErrorMappingService } from '../../../../../core/services/error.mapping.service';
import { UploadProgress, UploadResponse, DeleteResponse } from '../../../models/upload-image.interface';
import { CreateWebsiteCaseImageDto, WebsiteCaseImage } from '../../../models/website-case.interface';
import { ImageUploadData, ImageUploadModalComponent } from '../image-upload-modal/image-upload-modal.component';
import { ReorderImagesComponent } from '../reorder-images/reorder-image.component';


@Injectable({
  providedIn: 'root'
})
export class ModalService {
  private readonly dialog = inject(MatDialog);
  private readonly http = inject(HttpClient);
  private readonly errorMapping = inject(ErrorMappingService);

  openImageUploadModal(data?: ImageUploadData): Observable<CreateWebsiteCaseImageDto | undefined> {
    const dialogRef = this.dialog.open(ImageUploadModalComponent, {
      width: '600px',
      maxWidth: '90vw',
      maxHeight: '90vh',
      data: data || {},
      disableClose: true
    });

    return dialogRef.afterClosed();
  }

  uploadImage(file: File): Observable<UploadProgress> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<UploadResponse>(`${environment.apiUrl}/fileupload/image`, formData, {
      reportProgress: true,
      observe: 'events'
    }).pipe(
      map((event: HttpEvent<UploadResponse>) => {
        switch (event.type) {
          case HttpEventType.UploadProgress:
            if (event.total) {
              const progress = Math.round((event.loaded / event.total) * 100);
              return { progress };
            }
            return { progress: 0 };

          case HttpEventType.Response:
            if (event.body) {
              const response: UploadResponse = event.body;
              if (response.success) {
                return {
                  progress: 100,
                  imageUrl: response.imageUrl,
                  fileName: response.fileName,
                  originalName: response.originalName,
                  size: response.size
                };
              }
            }
            throw new Error('Upload failed');

          default:
            return { progress: 0 };
        }
      }),
      catchError((error) => {
        console.error('Upload error:', error);
        const errorMessage = this.errorMapping.mapServiceOrderError(error);
        return throwError(() => new Error(errorMessage));
      })
    );
  }

  openReorderImagesModal(images: WebsiteCaseImage[]): Observable<WebsiteCaseImage[] | undefined> {
    const dialogRef = this.dialog.open(ReorderImagesComponent, {
      width: '800px',
      maxWidth: '95vw',
      maxHeight: '90vh',
      data: { images },
      disableClose: true
    });

    return dialogRef.afterClosed();
  }

  deleteImage(fileName: string): Observable<boolean> {
    return this.http.delete<DeleteResponse>(`${environment.apiUrl}/fileupload/image/${fileName}`).pipe(
      map((response: DeleteResponse) => response.success),
      catchError((error) => {
        console.error('Delete error:', error);
        const errorMessage = this.errorMapping.mapServiceOrderError(error);
        return throwError(() => new Error(errorMessage));
      })
    );
  }
}