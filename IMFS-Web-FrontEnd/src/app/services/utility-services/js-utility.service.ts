import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { saveAs } from 'file-saver';

@Injectable()
export class JsUtilityService {
  constructor() {

  }

  intersect(a: string[], b: string[]) {
    const setA = new Set(a);
    const setB = new Set(b);
    const intersection = new Set(Array.from(setA).filter(x => setB.has(x)));
    return Array.from(intersection);
  }

  convertBlobToText(blob: Blob): Observable<string> {
    if (blob instanceof Blob === false) {
      return throwError('Unknown error');
    }
    const fileAsTextObservable = new Observable<string>(observer => {
      const reader = new FileReader();
      reader.onload = e => {
        const responseText = (e.target as any).result;

        observer.next(responseText);
        observer.complete();
      };
      const errMsg = reader.readAsText(blob, 'utf-8');
    });

    return fileAsTextObservable.pipe(
      switchMap(errMsgJsonAsText => {
        return throwError(JSON.parse(errMsgJsonAsText));
      })
    );
  }

  fileSaveAs(response: any) {
    const blob = response.body;
    // attachment; filename="FileName.xlsx"
    const re = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/; // get filename.xlsx from above
    const contentDisposition = response.headers.get('content-disposition');
    console.log(contentDisposition);

    let filename = '';
    if (contentDisposition) {
      filename = contentDisposition;
      const filenameArray = contentDisposition.match(re);
      if (filenameArray.length > 1) {
        filename = filenameArray[1].replace(/['"]/g, '');
      }
    }

    saveAs(blob, filename);
  }

  destroyCKEditor(editor: any) {
    setTimeout(() => {
      if (editor.instance) {
        editor.instance.removeAllListeners();
        editor.instance.destroy();
        editor.instance = null;
      }
    });
  }
}
