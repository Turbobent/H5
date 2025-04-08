class Common {
  int id;
  DateTime createdAt;
  DateTime updatedAt;

  Common({
    required this.id,
    required this.createdAt,
    required this.updatedAt,
  });

  factory Common.fromJson(Map<String, dynamic> json) {
    return Common(
      id: json['id'],
      createdAt: DateTime.parse(json['createdAt']),
      updatedAt: DateTime.parse(json['updatedAt']),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
    };
  }
}