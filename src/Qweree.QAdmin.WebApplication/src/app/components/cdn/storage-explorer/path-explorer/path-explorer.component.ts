import {Component, Input, OnChanges, OnInit, SimpleChanges} from '@angular/core';
import {ExplorerDirectory, ExplorerFile} from '../../../../model/cdn/ExplorerObject';
import {Observable} from 'rxjs';
import {UriHelper} from '../../../../services/UriHelper';
import {EnvironmentService} from '../../../../services/environment/environment.service';

@Component({
  selector: 'app-path-explorer',
  templateUrl: './path-explorer.component.html',
  styleUrls: ['./path-explorer.component.scss']
})
export class PathExplorerComponent implements OnInit, OnChanges {
  @Input() public path: string;
  @Input() public directoriesObservable: Observable<ExplorerDirectory[]>;
  @Input() public filesObservable: Observable<ExplorerFile[]>;

  public directoriesView: ExplorerDirectory[] = [];
  public filesView: ExplorerFile[] = [];
  public prevPath: string;
  public thisFolder = {
    totalCount: 0,
    totalSize: 0,
    itemsInView: 0
  };
  public orderField = 'filename';
  public orderDir = 1;

  constructor(
    private environmentService: EnvironmentService
  ) {
  }

  private static getPrevPath(path: string): string {
    let newPath = path;
    if (newPath.endsWith('/')) {
      newPath.substring(0, newPath.length - 1);
    }

    newPath = newPath.substring(0, newPath.lastIndexOf('/')) + '/';
    return newPath;
  }

  ngOnInit() {
    this.filesObservable.subscribe(files => {
      this.filesView = [];
      this.filesView = files.sort((a, b) => {
        const field = this.orderField;
        return a[field] > b[field] ? this.orderDir : this.orderDir * -1;
      });
      this.countThisFolder();
    });
    this.directoriesObservable.subscribe(directories => {
      this.directoriesView = [];
      this.directoriesView = directories.sort((a, b) => {
        let field = this.orderField;

        if (field === 'mediaType') { field = 'totalCount'; }
        if (field === 'size') { field = 'totalSize'; }

        return a[field] > b[field] ? this.orderDir : this.orderDir * -1;
      });
      this.countThisFolder();
    });
  }
  ngOnChanges(changes: SimpleChanges): void {
    this.prevPath = PathExplorerComponent.getPrevPath(this.path);
  }

  private sort(): void {
    this.filesView = this.filesView.sort((a, b) => {
      const field = this.orderField;
      return a[field] > b[field] ? this.orderDir : this.orderDir * -1;
    });
    this.directoriesView = this.directoriesView.sort((a, b) => {
      let field = this.orderField;

      if (field === 'mediaType') { field = 'totalCount'; }
      if (field === 'size') { field = 'totalSize'; }

      return a[field] > b[field] ? this.orderDir : this.orderDir * -1;
    });
  }

  sortBy(field: string): void {
    if (this.orderField === field) {
      this.orderDir *= -1;
      this.sort();
      return;
    }

    this.orderField = field;
    this.orderDir = 1;

    this.sort();
  }

  getHref(path: string): string|undefined {
    const uri = UriHelper.getUri(this.environmentService.getEnvironment().cdn.baseUri, '/api/v1/storage');
    return UriHelper.getUri(uri, path);
  }

  countThisFolder(): void {
    this.thisFolder = {
      totalCount: 0,
      totalSize: 0,
      itemsInView: 0
    };

    this.filesView.forEach(f => {
      this.thisFolder.itemsInView++;
      this.thisFolder.totalCount++;
      this.thisFolder.totalSize += f.size;
    });

    this.directoriesView.forEach(d => {
      this.thisFolder.itemsInView++;
      this.thisFolder.totalCount += d.totalCount;
      this.thisFolder.totalSize += d.totalSize;
    });
  }
}
