namespace VideoEditorBoilerplateGen.Templates;

public static class SwiftTemplates
{
    // ── App ────────────────────────────────────────────────────────────────

    public const string AppEntry = @"//  __PROJECT__App.swift
//  __PROJECT__

import SwiftUI

@main
struct __PROJECT__App: App {
    var body: some Scene {
        WindowGroup {
            ProjectListView()
        }
    }
}";

    // ── Domain / Entities ──────────────────────────────────────────────────

    public const string VideoProject = @"//  VideoProject.swift
//  __PROJECT__  —  Domain/Entities

import Foundation

struct VideoProject: Identifiable, Equatable {
    let id: UUID
    var name: String
    var createdAt: Date
    var updatedAt: Date
    var duration: TimeInterval
    var aspectRatio: AspectRatio
    var timeline: Timeline
    var exportSettings: ExportSettings

    init(
        id: UUID = UUID(),
        name: String,
        aspectRatio: AspectRatio = .widescreen,
        timeline: Timeline = Timeline()
    ) {
        self.id = id
        self.name = name
        self.createdAt = Date()
        self.updatedAt = Date()
        self.duration = 0
        self.aspectRatio = aspectRatio
        self.timeline = timeline
        self.exportSettings = ExportSettings()
    }
}

enum AspectRatio: String, CaseIterable {
    case widescreen = ""16:9""
    case portrait   = ""9:16""
    case square     = ""1:1""
    case cinema     = ""21:9""

    var size: CGSize {
        switch self {
        case .widescreen: return CGSize(width: 1920, height: 1080)
        case .portrait:   return CGSize(width: 1080, height: 1920)
        case .square:     return CGSize(width: 1080, height: 1080)
        case .cinema:     return CGSize(width: 2560, height: 1080)
        }
    }
}";

    public const string Timeline = @"//  Timeline.swift
//  __PROJECT__  —  Domain/Entities

import Foundation

struct Timeline: Equatable {
    var id: UUID
    var videoTracks: [VideoTrack]
    var audioTracks: [AudioTrack]
    var duration: TimeInterval

    init() {
        self.id = UUID()
        self.videoTracks = [VideoTrack()]
        self.audioTracks = []
        self.duration = 0
    }

    mutating func addClip(_ clip: VideoClip, to trackIndex: Int = 0) {
        guard trackIndex < videoTracks.count else { return }
        videoTracks[trackIndex].clips.append(clip)
        recalculateDuration()
    }

    mutating func removeClip(id: UUID) {
        for index in videoTracks.indices {
            videoTracks[index].clips.removeAll { $0.id == id }
        }
        recalculateDuration()
    }

    mutating func recalculateDuration() {
        duration = videoTracks.flatMap(\.clips)
            .map { $0.startTime + $0.duration }
            .max() ?? 0
    }
}";

    public const string VideoTrack = @"//  VideoTrack.swift
//  __PROJECT__  —  Domain/Entities

import Foundation

struct VideoTrack: Identifiable, Equatable {
    let id: UUID
    var clips: [VideoClip]
    var isMuted: Bool
    var volume: Float

    init() {
        self.id = UUID()
        self.clips = []
        self.isMuted = false
        self.volume = 1.0
    }
}";

    public const string VideoClip = @"//  VideoClip.swift
//  __PROJECT__  —  Domain/Entities

import Foundation

struct VideoClip: Identifiable, Equatable {
    let id: UUID
    var assetURL: URL
    var startTime: TimeInterval
    var duration: TimeInterval
    var trimStart: TimeInterval
    var trimEnd: TimeInterval
    var speed: Float
    var volume: Float
    var isMuted: Bool
    var effects: [Effect]
    var transform: ClipTransform

    init(assetURL: URL, duration: TimeInterval) {
        self.id = UUID()
        self.assetURL = assetURL
        self.startTime = 0
        self.duration = duration
        self.trimStart = 0
        self.trimEnd = 0
        self.speed = 1.0
        self.volume = 1.0
        self.isMuted = false
        self.effects = []
        self.transform = ClipTransform()
    }
}

struct ClipTransform: Equatable {
    var scale: CGFloat = 1.0
    var rotation: CGFloat = 0.0
    var position: CGPoint = .zero
    var opacity: Float = 1.0
}";

    public const string AudioTrack = @"//  AudioTrack.swift
//  __PROJECT__  —  Domain/Entities

import Foundation

struct AudioTrack: Identifiable, Equatable {
    let id: UUID
    var assetURL: URL
    var startTime: TimeInterval
    var duration: TimeInterval
    var volume: Float
    var isMuted: Bool
    var fadeIn: TimeInterval
    var fadeOut: TimeInterval

    init(assetURL: URL, duration: TimeInterval) {
        self.id = UUID()
        self.assetURL = assetURL
        self.startTime = 0
        self.duration = duration
        self.volume = 1.0
        self.isMuted = false
        self.fadeIn = 0
        self.fadeOut = 0
    }
}";

    public const string Effect = @"//  Effect.swift
//  __PROJECT__  —  Domain/Entities

import Foundation

struct Effect: Identifiable, Equatable {
    let id: UUID
    var type: EffectType
    var intensity: Float
    var startTime: TimeInterval
    var duration: TimeInterval

    init(type: EffectType, intensity: Float = 1.0) {
        self.id = UUID()
        self.type = type
        self.intensity = intensity
        self.startTime = 0
        self.duration = 0
    }
}

enum EffectType: String, CaseIterable {
    case brightness
    case contrast
    case saturation
    case warmth
    case vignette
    case blur
    case sharpen
    case fade
    case glitch
}";

    public const string ExportSettings = @"//  ExportSettings.swift
//  __PROJECT__  —  Domain/Entities

import Foundation

struct ExportSettings: Equatable {
    var resolution: ExportResolution
    var frameRate: Int
    var bitrate: Int
    var format: ExportFormat

    init() {
        self.resolution = .hd1080p
        self.frameRate = 30
        self.bitrate = 8_000_000
        self.format = .mp4
    }
}

enum ExportResolution: String, CaseIterable {
    case hd720p  = ""1280x720""
    case hd1080p = ""1920x1080""
    case uhd4k   = ""3840x2160""
}

enum ExportFormat: String, CaseIterable {
    case mp4
    case mov
}";

    // ── Domain / Repository Protocols ──────────────────────────────────────

    public const string VideoProjectRepositoryProtocol = @"//  VideoProjectRepositoryProtocol.swift
//  __PROJECT__  —  Domain/Repositories

import Foundation
import Combine

protocol VideoProjectRepositoryProtocol {
    func fetchAll() -> AnyPublisher<[VideoProject], Error>
    func fetch(id: UUID) -> AnyPublisher<VideoProject, Error>
    func save(_ project: VideoProject) -> AnyPublisher<Void, Error>
    func delete(id: UUID) -> AnyPublisher<Void, Error>
}";

    // ── Domain / Use Cases ─────────────────────────────────────────────────

    public const string UseCaseProtocols = @"//  UseCaseProtocols.swift
//  __PROJECT__  —  Domain/UseCases

import Foundation
import Combine
import AVFoundation

// MARK: - Fetch

protocol FetchProjectsUseCaseProtocol {
    func execute(input: Void) -> AnyPublisher<[VideoProject], Error>
}

final class FetchProjectsUseCase: FetchProjectsUseCaseProtocol {
    private let repository: VideoProjectRepositoryProtocol
    init(repository: VideoProjectRepositoryProtocol = DIContainer.shared.videoProjectRepository) {
        self.repository = repository
    }
    func execute(input: Void) -> AnyPublisher<[VideoProject], Error> {
        repository.fetchAll()
    }
}

// MARK: - Save

protocol SaveProjectUseCaseProtocol {
    func execute(input: VideoProject) -> AnyPublisher<Void, Error>
}

final class SaveProjectUseCase: SaveProjectUseCaseProtocol {
    private let repository: VideoProjectRepositoryProtocol
    init(repository: VideoProjectRepositoryProtocol = DIContainer.shared.videoProjectRepository) {
        self.repository = repository
    }
    func execute(input: VideoProject) -> AnyPublisher<Void, Error> {
        repository.save(input)
    }
}

// MARK: - Delete

protocol DeleteProjectUseCaseProtocol {
    func execute(input: UUID) -> AnyPublisher<Void, Error>
}

final class DeleteProjectUseCase: DeleteProjectUseCaseProtocol {
    private let repository: VideoProjectRepositoryProtocol
    init(repository: VideoProjectRepositoryProtocol = DIContainer.shared.videoProjectRepository) {
        self.repository = repository
    }
    func execute(input: UUID) -> AnyPublisher<Void, Error> {
        repository.delete(id: input)
    }
}

// MARK: - Export

protocol ExportProjectUseCaseProtocol {
    func execute(input: VideoProject) -> AnyPublisher<URL, Error>
}

final class ExportProjectUseCase: ExportProjectUseCaseProtocol {
    func execute(input: VideoProject) -> AnyPublisher<URL, Error> {
        Future<URL, Error> { promise in
            let composition = AVMutableComposition()
            guard let videoTrack = composition.addMutableTrack(withMediaType: .video, preferredTrackID: kCMPersistentTrackID_Invalid),
                  let audioTrack = composition.addMutableTrack(withMediaType: .audio, preferredTrackID: kCMPersistentTrackID_Invalid) else {
                promise(.failure(NSError(domain: ""ExportError"", code: -1, userInfo: [NSLocalizedDescriptionKey: ""Failed to create tracks""])))
                return
            }

            var currentTime = CMTime.zero
            
            for clip in input.timeline.videoTracks.flatMap({ $0.clips }) {
                let asset = AVURLAsset(url: clip.assetURL)
                let duration = CMTime(seconds: clip.duration, preferredTimescale: 600)
                let timeRange = CMTimeRange(start: .zero, duration: duration)
                
                do {
                    if let assetVideoTrack = try? asset.loadTracks(withMediaType: .video).first ?? asset.tracks(withMediaType: .video).first {
                        try videoTrack.insertTimeRange(timeRange, of: assetVideoTrack, at: currentTime)
                    }
                    if let assetAudioTrack = try? asset.loadTracks(withMediaType: .audio).first ?? asset.tracks(withMediaType: .audio).first {
                        try audioTrack.insertTimeRange(timeRange, of: assetAudioTrack, at: currentTime)
                    }
                    currentTime = CMTimeAdd(currentTime, duration)
                } catch {
                    print(""Failed to insert track: \(error)"")
                }
            }

            guard let session = AVAssetExportSession(asset: composition, presetName: AVAssetExportPresetHighestQuality) else {
                promise(.failure(NSError(domain: ""ExportError"", code: -2, userInfo: [NSLocalizedDescriptionKey: ""Failed to create export session""])))
                return
            }

            let outputURL = FileManager.default.temporaryDirectory.appendingPathComponent(""\(UUID().uuidString).mp4"")
            session.outputURL = outputURL
            session.outputFileType = .mp4

            session.exportAsynchronously {
                DispatchQueue.main.async {
                    switch session.status {
                    case .completed:
                        promise(.success(outputURL))
                    case .failed, .cancelled:
                        let err = session.error ?? NSError(domain: ""ExportError"", code: -3, userInfo: [NSLocalizedDescriptionKey: ""Export failed""])
                        promise(.failure(err))
                    default:
                        promise(.failure(NSError(domain: ""ExportError"", code: -4, userInfo: [NSLocalizedDescriptionKey: ""Unknown status""])))
                    }
                }
            }
        }.eraseToAnyPublisher()
    }
}";

    // ── Data / Repositories ────────────────────────────────────────────────

    public const string VideoProjectRepository = @"//  VideoProjectRepository.swift
//  __PROJECT__  —  Data/Repositories

import Foundation
import Combine

final class VideoProjectRepository: VideoProjectRepositoryProtocol {
    private let localDataSource: VideoProjectLocalDataSource

    init(localDataSource: VideoProjectLocalDataSource = VideoProjectLocalDataSource()) {
        self.localDataSource = localDataSource
    }

    func fetchAll() -> AnyPublisher<[VideoProject], Error> {
        localDataSource.fetchAll()
            .map { $0.map { $0.toDomain() } }
            .eraseToAnyPublisher()
    }

    func fetch(id: UUID) -> AnyPublisher<VideoProject, Error> {
        localDataSource.fetchAll()
            .tryMap {
                guard let dto = $0.first(where: { $0.id == id.uuidString }) else {
                    throw RepositoryError.notFound
                }
                return dto.toDomain()
            }
            .eraseToAnyPublisher()
    }

    func save(_ project: VideoProject) -> AnyPublisher<Void, Error> {
        localDataSource.save(VideoProjectDTO.fromDomain(project))
    }

    func delete(id: UUID) -> AnyPublisher<Void, Error> {
        localDataSource.delete(id: id)
    }
}

enum RepositoryError: LocalizedError {
    case notFound
    case saveFailed(Error)

    var errorDescription: String? {
        switch self {
        case .notFound:          return ""Record not found""
        case .saveFailed(let e): return ""Save failed: \(e.localizedDescription)""
        }
    }
}";

    // ── Data / DataSources ─────────────────────────────────────────────────

    public const string VideoProjectLocalDataSource = @"//  VideoProjectLocalDataSource.swift
//  __PROJECT__  —  Data/DataSources

import Foundation
import Combine

final class VideoProjectLocalDataSource {
    private let fileManager = FileManager.default

    private var projectsURL: URL {
        fileManager
            .urls(for: .documentDirectory, in: .userDomainMask)[0]
            .appendingPathComponent(""projects"", isDirectory: true)
    }

    init() {
        try? fileManager.createDirectory(at: projectsURL, withIntermediateDirectories: true)
    }

    func fetchAll() -> AnyPublisher<[VideoProjectDTO], Error> {
        Future { promise in
            do {
                let files = try self.fileManager
                    .contentsOfDirectory(at: self.projectsURL, includingPropertiesForKeys: nil)
                    .filter { $0.pathExtension == ""json"" }
                let decoder = JSONDecoder()
                decoder.dateDecodingStrategy = .iso8601
                let dtos = try files.map { try decoder.decode(VideoProjectDTO.self, from: Data(contentsOf: $0)) }
                promise(.success(dtos))
            } catch {
                promise(.failure(error))
            }
        }.eraseToAnyPublisher()
    }

    func save(_ dto: VideoProjectDTO) -> AnyPublisher<Void, Error> {
        Future { promise in
            do {
                let encoder = JSONEncoder()
                encoder.outputFormatting = .prettyPrinted
                encoder.dateEncodingStrategy = .iso8601
                let data = try encoder.encode(dto)
                let url = self.projectsURL.appendingPathComponent(""\(dto.id).json"")
                try data.write(to: url)
                promise(.success(()))
            } catch {
                promise(.failure(error))
            }
        }.eraseToAnyPublisher()
    }

    func delete(id: UUID) -> AnyPublisher<Void, Error> {
        Future { promise in
            let url = self.projectsURL.appendingPathComponent(""\(id.uuidString).json"")
            do {
                try self.fileManager.removeItem(at: url)
                promise(.success(()))
            } catch {
                promise(.failure(error))
            }
        }.eraseToAnyPublisher()
    }
}";

    // ── Data / DTOs ────────────────────────────────────────────────────────

    public const string VideoProjectDTO = @"//  VideoProjectDTO.swift
//  __PROJECT__  —  Data/DTOs

import Foundation

struct VideoProjectDTO: Codable {
    let id: String
    var name: String
    var createdAt: Date
    var updatedAt: Date
    var duration: TimeInterval
    var aspectRatio: String
    var timeline: TimelineDTO
    var exportSettings: ExportSettingsDTO

    func toDomain() -> VideoProject {
        var project = VideoProject(
            id: UUID(uuidString: id) ?? UUID(),
            name: name,
            aspectRatio: AspectRatio(rawValue: aspectRatio) ?? .widescreen,
            timeline: timeline.toDomain()
        )
        project.createdAt = createdAt
        project.updatedAt = updatedAt
        project.duration = duration
        project.exportSettings = exportSettings.toDomain()
        return project
    }

    static func fromDomain(_ entity: VideoProject) -> VideoProjectDTO {
        VideoProjectDTO(
            id: entity.id.uuidString,
            name: entity.name,
            createdAt: entity.createdAt,
            updatedAt: entity.updatedAt,
            duration: entity.duration,
            aspectRatio: entity.aspectRatio.rawValue,
            timeline: TimelineDTO.fromDomain(entity.timeline),
            exportSettings: ExportSettingsDTO.fromDomain(entity.exportSettings)
        )
    }
}

struct TimelineDTO: Codable {
    var id: String
    var videoTracks: [VideoTrackDTO]
    var audioTracks: [AudioTrackDTO]
    var duration: TimeInterval

    func toDomain() -> Timeline {
        var timeline = Timeline()
        timeline.id = UUID(uuidString: id) ?? UUID()
        timeline.videoTracks = videoTracks.map { $0.toDomain() }
        timeline.audioTracks = audioTracks.map { $0.toDomain() }
        timeline.duration = duration
        return timeline
    }

    static func fromDomain(_ entity: Timeline) -> TimelineDTO {
        TimelineDTO(
            id: entity.id.uuidString,
            videoTracks: entity.videoTracks.map { VideoTrackDTO.fromDomain($0) },
            audioTracks: entity.audioTracks.map { AudioTrackDTO.fromDomain($0) },
            duration: entity.duration
        )
    }
}

struct VideoTrackDTO: Codable {
    let id: String
    var clips: [VideoClipDTO]
    var isMuted: Bool
    var volume: Float

    func toDomain() -> VideoTrack {
        var track = VideoTrack()
        track.clips = clips.map { $0.toDomain() }
        track.isMuted = isMuted
        track.volume = volume
        return track
    }

    static func fromDomain(_ entity: VideoTrack) -> VideoTrackDTO {
        VideoTrackDTO(
            id: entity.id.uuidString,
            clips: entity.clips.map { VideoClipDTO.fromDomain($0) },
            isMuted: entity.isMuted,
            volume: entity.volume
        )
    }
}

struct VideoClipDTO: Codable {
    let id: String
    var assetURL: String
    var startTime: TimeInterval
    var duration: TimeInterval
    var trimStart: TimeInterval
    var trimEnd: TimeInterval
    var speed: Float
    var volume: Float
    var isMuted: Bool
    var effects: [EffectDTO]
    var transform: ClipTransformDTO

    func toDomain() -> VideoClip {
        var clip = VideoClip(assetURL: URL(string: assetURL) ?? URL(fileURLWithPath: """"), duration: duration)
        clip.startTime = startTime
        clip.trimStart = trimStart
        clip.trimEnd = trimEnd
        clip.speed = speed
        clip.volume = volume
        clip.isMuted = isMuted
        clip.effects = effects.map { $0.toDomain() }
        clip.transform = transform.toDomain()
        return clip
    }

    static func fromDomain(_ entity: VideoClip) -> VideoClipDTO {
        VideoClipDTO(
            id: entity.id.uuidString,
            assetURL: entity.assetURL.absoluteString,
            startTime: entity.startTime,
            duration: entity.duration,
            trimStart: entity.trimStart,
            trimEnd: entity.trimEnd,
            speed: entity.speed,
            volume: entity.volume,
            isMuted: entity.isMuted,
            effects: entity.effects.map { EffectDTO.fromDomain($0) },
            transform: ClipTransformDTO.fromDomain(entity.transform)
        )
    }
}

struct AudioTrackDTO: Codable {
    let id: String
    var assetURL: String
    var startTime: TimeInterval
    var duration: TimeInterval
    var volume: Float
    var isMuted: Bool
    var fadeIn: TimeInterval
    var fadeOut: TimeInterval

    func toDomain() -> AudioTrack {
        var track = AudioTrack(assetURL: URL(string: assetURL) ?? URL(fileURLWithPath: """"), duration: duration)
        track.startTime = startTime
        track.volume = volume
        track.isMuted = isMuted
        track.fadeIn = fadeIn
        track.fadeOut = fadeOut
        return track
    }

    static func fromDomain(_ entity: AudioTrack) -> AudioTrackDTO {
        AudioTrackDTO(
            id: entity.id.uuidString,
            assetURL: entity.assetURL.absoluteString,
            startTime: entity.startTime,
            duration: entity.duration,
            volume: entity.volume,
            isMuted: entity.isMuted,
            fadeIn: entity.fadeIn,
            fadeOut: entity.fadeOut
        )
    }
}

struct EffectDTO: Codable {
    let id: String
    var type: String
    var intensity: Float
    var startTime: TimeInterval
    var duration: TimeInterval

    func toDomain() -> Effect {
        var effect = Effect(type: EffectType(rawValue: type) ?? .brightness, intensity: intensity)
        effect.startTime = startTime
        effect.duration = duration
        return effect
    }

    static func fromDomain(_ entity: Effect) -> EffectDTO {
        EffectDTO(
            id: entity.id.uuidString,
            type: entity.type.rawValue,
            intensity: entity.intensity,
            startTime: entity.startTime,
            duration: entity.duration
        )
    }
}

struct ClipTransformDTO: Codable {
    var scale: CGFloat
    var rotation: CGFloat
    var positionX: CGFloat
    var positionY: CGFloat
    var opacity: Float

    func toDomain() -> ClipTransform {
        ClipTransform(scale: scale, rotation: rotation, position: CGPoint(x: positionX, y: positionY), opacity: opacity)
    }

    static func fromDomain(_ entity: ClipTransform) -> ClipTransformDTO {
        ClipTransformDTO(scale: entity.scale, rotation: entity.rotation, positionX: entity.position.x, positionY: entity.position.y, opacity: entity.opacity)
    }
}

struct ExportSettingsDTO: Codable {
    var resolution: String
    var frameRate: Int
    var bitrate: Int
    var format: String

    func toDomain() -> ExportSettings {
        var settings = ExportSettings()
        settings.resolution = ExportResolution(rawValue: resolution) ?? .hd1080p
        settings.frameRate = frameRate
        settings.bitrate = bitrate
        settings.format = ExportFormat(rawValue: format) ?? .mp4
        return settings
    }

    static func fromDomain(_ entity: ExportSettings) -> ExportSettingsDTO {
        ExportSettingsDTO(
            resolution: entity.resolution.rawValue,
            frameRate: entity.frameRate,
            bitrate: entity.bitrate,
            format: entity.format.rawValue
        )
    }
}";

    // ── Core / DI ──────────────────────────────────────────────────────────

    public const string DIContainer = @"//  DIContainer.swift
//  __PROJECT__  —  Core/DI

import Foundation

final class DIContainer {
    static let shared = DIContainer()
    private init() {}

    // Data Sources
    lazy var videoProjectLocalDataSource = VideoProjectLocalDataSource()

    // Repositories
    lazy var videoProjectRepository: VideoProjectRepositoryProtocol =
        VideoProjectRepository(localDataSource: videoProjectLocalDataSource)

    // Use Cases
    lazy var fetchProjectsUseCase: FetchProjectsUseCaseProtocol =
        FetchProjectsUseCase(repository: videoProjectRepository)

    lazy var saveProjectUseCase: SaveProjectUseCaseProtocol =
        SaveProjectUseCase(repository: videoProjectRepository)

    lazy var deleteProjectUseCase: DeleteProjectUseCaseProtocol =
        DeleteProjectUseCase(repository: videoProjectRepository)

    lazy var exportProjectUseCase: ExportProjectUseCaseProtocol =
        ExportProjectUseCase()
}";

    // ── Presentation / ProjectList ─────────────────────────────────────────

    public const string ProjectListViewModel = @"//  ProjectListViewModel.swift
//  __PROJECT__  —  Presentation/Features/ProjectList

import Foundation
import Combine

@MainActor
final class ProjectListViewModel: ObservableObject {
    @Published var projects: [VideoProject] = []
    @Published var isLoading: Bool = false
    @Published var errorMessage: String? = nil

    private let fetchProjectsUseCase: FetchProjectsUseCaseProtocol
    private let deleteProjectUseCase: DeleteProjectUseCaseProtocol
    private var cancellables = Set<AnyCancellable>()

    init(
        fetchProjectsUseCase: FetchProjectsUseCaseProtocol = DIContainer.shared.fetchProjectsUseCase,
        deleteProjectUseCase: DeleteProjectUseCaseProtocol = DIContainer.shared.deleteProjectUseCase
    ) {
        self.fetchProjectsUseCase = fetchProjectsUseCase
        self.deleteProjectUseCase = deleteProjectUseCase
    }

    func loadProjects() {
        isLoading = true
        fetchProjectsUseCase.execute(input: ())
            .receive(on: DispatchQueue.main)
            .sink(
                receiveCompletion: { [weak self] completion in
                    self?.isLoading = false
                    if case .failure(let error) = completion {
                        self?.errorMessage = error.localizedDescription
                    }
                },
                receiveValue: { [weak self] in self?.projects = $0 }
            )
            .store(in: &cancellables)
    }

    func deleteProject(id: UUID) {
        deleteProjectUseCase.execute(input: id)
            .receive(on: DispatchQueue.main)
            .sink(
                receiveCompletion: { [weak self] completion in
                    if case .failure(let error) = completion {
                        self?.errorMessage = error.localizedDescription
                    }
                },
                receiveValue: { [weak self] _ in
                    self?.projects.removeAll { $0.id == id }
                }
            )
            .store(in: &cancellables)
    }
}";

    public const string ProjectListView = @"//  ProjectListView.swift
//  __PROJECT__  —  Presentation/Features/ProjectList

import SwiftUI

struct ProjectListView: View {
    @StateObject private var viewModel = ProjectListViewModel()
    @State private var showingNewProject = false

    var body: some View {
        NavigationStack {
            Group {
                if viewModel.isLoading {
                    ProgressView()
                } else if viewModel.projects.isEmpty {
                    emptyState
                } else {
                    projectGrid
                }
            }
            .navigationTitle(""__PROJECT__"")
            .toolbar {
                ToolbarItem(placement: .primaryAction) {
                    Button { showingNewProject = true } label: {
                        Image(systemName: ""plus"")
                    }
                }
            }
            .sheet(isPresented: $showingNewProject) {
                NewProjectView()
            }
            .alert(""Error"", isPresented: .constant(viewModel.errorMessage != nil)) {
                Button(""OK"") { viewModel.errorMessage = nil }
            } message: {
                Text(viewModel.errorMessage ?? """")
            }
        }
        .onAppear { viewModel.loadProjects() }
    }

    private var emptyState: some View {
        VStack(spacing: 16) {
            Image(systemName: ""film.stack"")
                .font(.system(size: 64))
                .foregroundStyle(.secondary)
            Text(""No Projects"")
                .font(.title2.bold())
            Text(""Tap + to create your first project"")
                .foregroundStyle(.secondary)
            Button(""New Project"") { showingNewProject = true }
                .buttonStyle(.borderedProminent)
        }
    }

    private var projectGrid: some View {
        ScrollView {
            LazyVGrid(columns: [GridItem(.adaptive(minimum: 160))], spacing: 16) {
                ForEach(viewModel.projects) { project in
                    NavigationLink(destination: EditorView(project: project)) {
                        ProjectThumbnailView(project: project)
                    }
                    .contextMenu {
                        Button(role: .destructive) {
                            viewModel.deleteProject(id: project.id)
                        } label: {
                            Label(""Delete"", systemImage: ""trash"")
                        }
                    }
                }
            }
            .padding()
        }
    }
}

#Preview { ProjectListView() }";

    // ── Presentation / Editor ──────────────────────────────────────────────

    public const string EditorViewModel = @"//  EditorViewModel.swift
//  __PROJECT__  —  Presentation/Features/Editor

import Foundation
import Combine
import AVFoundation

@MainActor
final class EditorViewModel: ObservableObject {
    @Published var project: VideoProject
    @Published var selectedClipId: UUID? = nil
    @Published var currentTime: TimeInterval = 0
    @Published var isPlaying: Bool = false
    @Published var isLoading: Bool = false
    @Published var errorMessage: String? = nil

    private let saveProjectUseCase: SaveProjectUseCaseProtocol
    private let exportProjectUseCase: ExportProjectUseCaseProtocol
    private var cancellables = Set<AnyCancellable>()

    var selectedClip: VideoClip? {
        guard let id = selectedClipId else { return nil }
        return project.timeline.videoTracks.flatMap(\.clips).first { $0.id == id }
    }

    init(
        project: VideoProject,
        saveProjectUseCase: SaveProjectUseCaseProtocol = DIContainer.shared.saveProjectUseCase,
        exportProjectUseCase: ExportProjectUseCaseProtocol = DIContainer.shared.exportProjectUseCase
    ) {
        self.project = project
        self.saveProjectUseCase = saveProjectUseCase
        self.exportProjectUseCase = exportProjectUseCase
    }

    func addClip(url: URL) {
        Task {
            let asset = AVURLAsset(url: url)
            let duration = try? await asset.load(.duration)
            let clip = VideoClip(assetURL: url, duration: duration?.seconds ?? 0)
            project.timeline.addClip(clip)
            saveProject()
        }
    }

    func removeClip(id: UUID) {
        project.timeline.removeClip(id: id)
        if selectedClipId == id { selectedClipId = nil }
        saveProject()
    }

    func selectClip(id: UUID) {
        selectedClipId = (selectedClipId == id) ? nil : id
    }

    func exportProject() {
        isLoading = true
        exportProjectUseCase.execute(input: project)
            .receive(on: DispatchQueue.main)
            .sink(
                receiveCompletion: { [weak self] completion in
                    self?.isLoading = false
                    if case .failure(let error) = completion {
                        self?.errorMessage = error.localizedDescription
                    }
                },
                receiveValue: { _ in }
            )
            .store(in: &cancellables)
    }

    private func saveProject() {
        saveProjectUseCase.execute(input: project)
            .sink(receiveCompletion: { _ in }, receiveValue: { _ in })
            .store(in: &cancellables)
    }
}";

    public const string EditorView = @"//  EditorView.swift
//  __PROJECT__  —  Presentation/Features/Editor

import SwiftUI

struct EditorView: View {
    @StateObject private var viewModel: EditorViewModel
    @State private var showingExport = false

    init(project: VideoProject) {
        _viewModel = StateObject(wrappedValue: EditorViewModel(project: project))
    }

    var body: some View {
        VStack(spacing: 0) {
            VideoPreviewView(
                project: viewModel.project,
                currentTime: viewModel.currentTime,
                isPlaying: viewModel.isPlaying
            )
            .frame(maxHeight: 300)

            PlaybackControlsView(
                isPlaying: $viewModel.isPlaying,
                currentTime: $viewModel.currentTime,
                duration: viewModel.project.timeline.duration
            )

            Divider()

            TimelineView(
                timeline: viewModel.project.timeline,
                currentTime: viewModel.currentTime,
                selectedClipId: viewModel.selectedClipId,
                onSelectClip: { viewModel.selectClip(id: $0) },
                onDeleteClip: { viewModel.removeClip(id: $0) }
            )
            .frame(height: 120)

            if let clip = viewModel.selectedClip {
                ClipToolsView(clip: clip, onUpdateEffect: { _ in })
            }
        }
        .navigationTitle(viewModel.project.name)
        .navigationBarTitleDisplayMode(.inline)
        .toolbar {
            ToolbarItemGroup(placement: .primaryAction) {
                Button { viewModel.exportProject() } label: {
                    Image(systemName: ""square.and.arrow.up"")
                }
            }
        }
    }
}";

    // ── Presentation / Components ──────────────────────────────────────────

    public const string TimelineView = @"//  TimelineView.swift
//  __PROJECT__  —  Presentation/Features/Editor/Components

import SwiftUI

struct TimelineView: View {
    let timeline: Timeline
    let currentTime: TimeInterval
    let selectedClipId: UUID?
    let onSelectClip: (UUID) -> Void
    let onDeleteClip: (UUID) -> Void

    private let pixelsPerSecond: CGFloat = 60

    var body: some View {
        ScrollView(.horizontal, showsIndicators: false) {
            ZStack(alignment: .topLeading) {
                VStack(spacing: 2) {
                    ForEach(timeline.videoTracks) { track in
                        VideoTrackRowView(
                            track: track,
                            pixelsPerSecond: pixelsPerSecond,
                            selectedClipId: selectedClipId,
                            onSelectClip: onSelectClip,
                            onDeleteClip: onDeleteClip
                        )
                    }
                }
                // Playhead
                Rectangle()
                    .fill(.red)
                    .frame(width: 2)
                    .offset(x: CGFloat(currentTime) * pixelsPerSecond)
            }
            .padding(.horizontal)
        }
        .background(Color(uiColor: .systemGray6))
    }
}";

    public const string StubViews = @"//  StubViews.swift
//  __PROJECT__  —  Presentation/Components
//  Placeholder views — replace with real implementations

import SwiftUI
import AVFoundation

struct VideoPreviewView: UIViewRepresentable {
    let project: VideoProject
    let currentTime: TimeInterval
    let isPlaying: Bool

    func makeUIView(context: Context) -> PlayerUIView {
        PlayerUIView(frame: .zero)
    }

    func updateUIView(_ uiView: PlayerUIView, context: Context) {
        uiView.update(with: project, currentTime: currentTime, isPlaying: isPlaying)
    }
}

class PlayerUIView: UIView {
    private var player: AVPlayer?
    private var playerLayer: AVPlayerLayer {
        layer as! AVPlayerLayer
    }

    override static var layerClass: AnyClass {
        AVPlayerLayer.self
    }

    override init(frame: CGRect) {
        super.init(frame: frame)
        backgroundColor = .black
        playerLayer.videoGravity = .resizeAspect
    }

    required init?(coder: NSCoder) {
        fatalError(""init(coder:) has not been implemented"")
    }

    func update(with project: VideoProject, currentTime: TimeInterval, isPlaying: Bool) {
        if player == nil {
            if let firstClip = project.timeline.videoTracks.first?.clips.first {
                player = AVPlayer(url: firstClip.assetURL)
                playerLayer.player = player
            }
        }

        if isPlaying {
            player?.play()
        } else {
            player?.pause()
            let targetTime = CMTime(seconds: currentTime, preferredTimescale: 600)
            player?.seek(to: targetTime, toleranceBefore: .zero, toleranceAfter: .zero)
        }
    }
}

struct PlaybackControlsView: View {
    @Binding var isPlaying: Bool
    @Binding var currentTime: TimeInterval
    let duration: TimeInterval

    var body: some View {
        HStack(spacing: 24) {
            Button { currentTime = max(0, currentTime - 5) } label: {
                Image(systemName: ""gobackward.5"")
            }
            Button { isPlaying.toggle() } label: {
                Image(systemName: isPlaying ? ""pause.fill"" : ""play.fill"")
                    .font(.title2)
            }
            Button { currentTime = min(duration, currentTime + 5) } label: {
                Image(systemName: ""goforward.5"")
            }
        }
        .padding()
    }
}

struct VideoTrackRowView: View {
    let track: VideoTrack
    let pixelsPerSecond: CGFloat
    let selectedClipId: UUID?
    let onSelectClip: (UUID) -> Void
    let onDeleteClip: (UUID) -> Void

    var body: some View {
        HStack(spacing: 2) {
            ForEach(track.clips) { clip in
                RoundedRectangle(cornerRadius: 4)
                    .fill(selectedClipId == clip.id ? Color.blue : Color.gray)
                    .frame(width: CGFloat(clip.duration) * pixelsPerSecond, height: 60)
                    .onTapGesture { onSelectClip(clip.id) }
                    .contextMenu {
                        Button(role: .destructive) {
                            onDeleteClip(clip.id)
                        } label: {
                            Label(""Delete"", systemImage: ""trash"")
                        }
                    }
            }
        }
    }
}

struct ClipToolsView: View {
    let clip: VideoClip
    let onUpdateEffect: (Effect) -> Void

    var body: some View {
        ScrollView(.horizontal, showsIndicators: false) {
            HStack(spacing: 12) {
                ForEach(EffectType.allCases, id: \.self) { effectType in
                    Button(effectType.rawValue.capitalized) {
                        onUpdateEffect(Effect(type: effectType, intensity: 0.5))
                    }
                    .buttonStyle(.bordered)
                }
            }
            .padding()
        }
    }
}

struct NewProjectView: View {
    @Environment(\.dismiss) var dismiss
    @State private var name: String = """"
    @State private var aspectRatio: AspectRatio = .widescreen
    
    private let saveUseCase: SaveProjectUseCaseProtocol = DIContainer.shared.saveProjectUseCase

    var body: some View {
        NavigationStack {
            Form {
                Section(""Project Name"") {
                    TextField(""My Video"", text: $name)
                }
                Section(""Aspect Ratio"") {
                    Picker(""Ratio"", selection: $aspectRatio) {
                        ForEach(AspectRatio.allCases, id: \.self) {
                            Text($0.rawValue).tag($0)
                        }
                    }
                    .pickerStyle(.segmented)
                }
            }
            .navigationTitle(""New Project"")
            .toolbar {
                ToolbarItem(placement: .cancellationAction) {
                    Button(""Cancel"") { dismiss() }
                }
                ToolbarItem(placement: .confirmationAction) {
                    Button(""Create"") {
                        let newProject = VideoProject(name: name, aspectRatio: aspectRatio)
                        _ = saveUseCase.execute(input: newProject)
                        dismiss()
                    }
                    .disabled(name.isEmpty)
                }
            }
            .preferredColorScheme(.dark)
            .tint(.orange)
        }
    }
}

struct ProjectThumbnailView: View {
    let project: VideoProject

    var body: some View {
        VStack(alignment: .leading, spacing: 8) {
            Rectangle()
                .fill(Color.gray.opacity(0.3))
                .aspectRatio(16/9, contentMode: .fit)
                .cornerRadius(8)
                .overlay(
                    Image(systemName: ""film"")
                        .foregroundStyle(.secondary)
                )
            Text(project.name)
                .font(.caption.bold())
                .lineLimit(1)
            Text(project.aspectRatio.rawValue)
                .font(.caption2)
                .foregroundStyle(.secondary)
    }
}";

    // ── Resouce Generators ──────────────────────────────────────────────────

    public const string GenericView = @"//  __NAME__.swift
//  Generated by vegen

import SwiftUI

struct __NAME__: View {
    var body: some View {
        VStack {
            Text(""__NAME__"")
                .font(.headline)
        }
        .preferredColorScheme(.dark)
    }
}

#Preview {
    __NAME__()
}";

    public const string GenericModel = @"//  __NAME__.swift
//  Generated by vegen

import Foundation

struct __NAME__: Identifiable, Equatable, Codable {
    let id: UUID
    
    init(id: UUID = UUID()) {
        self.id = id
    }
}";

    public const string GenericFile = @"//  __NAME__.swift
//  Generated by vegen

import Foundation

// TODO: Implement __NAME__
";

    public const string AppTheme = @"//  __NAME__.swift
//  Generated by vegen
//  Clean Architecture Theme Definitions

import SwiftUI

enum AppTheme {
    static let primary = Color.orange
    static let secondary = Color.gray
    
    enum Colors {
        static let background = Color(""Background"")
        static let surfaceHigh = Color(""SurfaceHigh"")
        static let surfaceLow = Color(""SurfaceLow"")
        static let textPrimary = Color(""TextPrimary"")
        static let textSecondary = Color(""TextSecondary"")
    }

    enum Typography {
        static let displayLarge = Font.system(size: 32, weight: .bold, design: .default)
        static let headlineMedium = Font.system(size: 24, weight: .semibold, design: .default)
        static let bodyDefault = Font.system(size: 16, weight: .regular, design: .default)
        static let labelCaps = Font.system(size: 12, weight: .medium, design: .default).smallCaps()
    }
}

// Preview color scheme testing stub
#Preview {
    VStack(spacing: 20) {
        Text(""Light Mode"").environment(\.colorScheme, .light)
        Text(""Dark Mode"").environment(\.colorScheme, .dark)
    }
    .font(AppTheme.Typography.headlineMedium)
    .foregroundColor(AppTheme.primary)
}";

    public const string AssetsContents = @"{
  ""info"" : {
    ""author"" : ""xcode"",
    ""version"" : 1
  }
}";

    public const string AppIconContents = @"{
  ""images"" : [
    {
      ""idiom"" : ""universal"",
      ""platform"" : ""ios"",
      ""size"" : ""1024x1024""
    }
  ],
  ""info"" : {
    ""author"" : ""xcode"",
    ""version"" : 1
  }
}";

    public const string AccentColorContents = @"{
  ""colors"" : [
    {
      ""idiom"" : ""universal"",
      ""color"" : {
        ""color-space"" : ""srgb"",
        ""components"" : {
          ""alpha"" : ""1.000"",
          ""blue"" : ""0.000"",
          ""green"" : ""0.584"",
          ""red"" : ""1.000""
        }
      }
    }
  ],
  ""info"" : {
    ""author"" : ""xcode"",
    ""version"" : 1
  }
}";
}
