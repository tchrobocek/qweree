import {Component, OnInit} from '@angular/core';
import {CdnAdapterService} from '../../../services/cdn/cdn-adapter.service';
import {NavigationEnd, Router} from '@angular/router';
import {ExplorerDirectory, ExplorerFile} from '../../../model/cdn/ExplorerObject';
import {Subject} from 'rxjs';
import {UriHelper} from '../../../services/UriHelper';
import {EnvironmentService} from '../../../services/environment/environment.service';

@Component({
  selector: 'app-storage-explorer',
  templateUrl: './storage-explorer.component.html',
  styleUrls: ['./storage-explorer.component.scss']
})
export class StorageExplorerComponent implements OnInit {

  private currentPathSubject = new Subject<string>();
  private currentDirectoriesSubject = new Subject<ExplorerDirectory[]>();
  private currentFilesSubject = new Subject<ExplorerFile[]>();

  public currentPathObservable = this.currentPathSubject.asObservable();
  public currentDirectoriesObservable = this.currentDirectoriesSubject.asObservable();
  public currentFilesObservable = this.currentFilesSubject.asObservable();

  public path: string;
  public inputPath: string;

  constructor(
    private cdnAdapter: CdnAdapterService,
    public router: Router,
    private environmentService: EnvironmentService
  ) {
  }

  private static getFilename(path: string): string {
    if (path.endsWith('/')) {
      path = path.substr(0, path.length - 1);
    }

    return path.substr(path.lastIndexOf('/') + 1);
  }

  private static getPathFromUri(uri: string): string {
    let path = uri.substring('/cdn/explorer'.length);

    if (!path) {
      path = '/';
    }

    return path;
  }

  ngOnInit(): void {
    this.itemsLoader();
    this.router.events.subscribe(e => {
      if (e.constructor.name === 'NavigationEnd') {
        this.updatePath(StorageExplorerComponent.getPathFromUri((e as NavigationEnd).url));
      }
    });
    this.updatePath(StorageExplorerComponent.getPathFromUri(this.router.url));
  }

  itemsLoader(): void {
    this.currentPathObservable.subscribe(currentPath => {

      this.currentDirectoriesSubject.next([]);
      this.currentFilesSubject.next([]);
      this.cdnAdapter.explore(currentPath)
        .subscribe(objects => {
          const files = [];
          const directories = [];

          objects.forEach(o => {
            const dir = (o as ExplorerDirectory);
            if (dir.totalCount) {
              dir.filename = StorageExplorerComponent.getFilename(dir.path);
              directories.push(dir);
            }
            const file = (o as ExplorerFile);
            if (file.mediaType) {
              file.filename = StorageExplorerComponent.getFilename(file.path);
              files.push(file);
            }
          });

          this.currentFilesSubject.next(files);
          this.currentDirectoriesSubject.next(directories);
        });
    });
  }

  updatePath(path: string): void {
    while (path.endsWith('/')) {
      path = path.substr(0, path.length - 1);
    }
    if (path === '') {
      path = '/';
    }

    if (this.path === path) {
      return;
    }

    this.path = path.toString();
    this.inputPath = this.path;
    this.currentPathSubject.next(path);
  }

  getCdnBaseUrl(): string {
    return UriHelper.getUri(this.environmentService.getEnvironment().cdn.baseUri, '/api/v1/storage');
  }
}
